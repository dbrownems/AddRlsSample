using Microsoft.AnalysisServices.Tabular;

var workspaceName = "dbrowne";
var memberName = "marcsteam@microsoft.com";
var xmlaEndpoint = $"powerbi://api.powerbi.com/v1.0/myorg/{workspaceName}";
var datasetName = "AddRlsSample";
var roleName = "South Region";
var tableName = "Accounts";
var rlsFilter = @"[Region] = ""South""";


var svr = new Microsoft.AnalysisServices.Server();
svr.Connect($"Data Source={xmlaEndpoint};Initial Catalog={datasetName}"); //interactive auth.  Use Service Principal in real life
var model = svr.Databases.FindByName(datasetName).Model;

AddRlsRole(model, datasetName, roleName, tableName, rlsFilter, memberName);

static void AddRlsRole(Model model, string datasetName, string roleName, string tableName, string rlsFilter,  string memberName)
{
   
    ModelRoleCollection roles = model.Roles;

    if (roles.Contains(roleName))
    {
        roles.Remove(roleName);
        model.SaveChanges();
    }

    var role = new ModelRole() { Name = roleName };
    role.ModelPermission = ModelPermission.Read;

    var table = model.Tables[tableName];
    var perm = new TablePermission() { Table = table, FilterExpression = rlsFilter };
    role.TablePermissions.Add(perm);

    var member = new ExternalModelRoleMember();
    member.MemberName = memberName;
    member.MemberType = RoleMemberType.Auto;
    member.IdentityProvider = "AzureAD";
    role.Members.Add(member);


    model.Roles.Add(role);

    model.SaveChanges();
}

