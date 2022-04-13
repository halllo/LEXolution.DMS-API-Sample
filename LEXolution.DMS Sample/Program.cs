using STP.Ecm.ApiCore;
using STP.Ecm.Dto.Container;

var config = new Config();
var dms = new Dms(config);
try
{
    Console.WriteLine("Login...");
    var initResult = dms.Init(config.UmServerUser, config.UmPassword);

    Console.WriteLine("Load container...");
    var containerId = (await dms.Container.SearchContainersByString("%")).Take(1).ToList();
    var container = (await dms.Container.LoadContainers(containerId)).First();
    Console.WriteLine(container switch
    {
        DmsDossierDto dossier => $"{dossier.AzIntern} {dossier.Bezeichnung} (Dossier)",
        DmsFolderDto folder => $"{folder.Name} (Folder)",
        _ => string.Empty
    });

    Console.WriteLine("Loading documents...");
    var documentIds = (await dms.Container.GetDocumentIdsForContainer(container.Id)).Take(50).ToList();
    var documents = await dms.Document.LoadDocumentData(documentIds);
    foreach (var document in documents)
    {
        Console.WriteLine(document.Title);
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
}
finally
{
    dms.Dispose();
}



public class Config : ILsbConfiguration
{
    public string RabbitMqHostname { get; set; } = "";
    public int RabbitMqPort { get; set; } = 5672;
    public string RabbitMqUsername { get; set; } = "";
    public string RabbitMqPassword { get; set; } = "";
    public string UmServerUser { get; set; } = "";
    public string UmPassword { get; set; } = "";
}
