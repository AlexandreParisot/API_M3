# API_M3
Projet connecteur API M3 en .net 6 <br/>
Libraire de communication avec les API M3.

Exemple utilisation :
```c#
//Déclaration des class utilisées.

public class MNS260MI_CRA
{
    public string DONR { get; set; }
    public string PRF1 { get; set; }
    public string PRTF { get; set; }
    public string OBJC { get; set; }
    public string DONO { get; set; }
    public string MKV4 { get; set; }
    public string MKV5 { get; set; }
    public string MKV6 { get; set; }
    public string MKV7 { get; set; }
}

public class MNS260MI_GUID
{
    public string BMIN { get; set; }
}

 M3Client.SetConnectorM3(_config["ApiM3:User"], _config["ApiM3:Pwd"], _config["ApiM3:Url"]);
 var res = M3Client.GetData<MNS260MI_GUID>("MNS260MI", "AddMBMInit", new MNS260MI_CRA() { DONR = "MBM", PRF1 = "IDG_CRA", PRTF="IDG_CRA",DONO = depot, OBJC = "DLIX", MKV4 = produit, MKV5= dateCrea, MKV6 = heureCrea, MKV7= Sufixe   });
 if (res.Success) {    
     if (res.Data != null)
     {
         if ((res.Data is List<MNS260MI_GUID> guid) && !string.IsNullOrEmpty(guid.First().BMIN))
         {
           console.write(   guid.First().BMIN);
         }
     }
}
```

Retour un objet M3Reponse. Il retourne la donnée sous trois formats :
 -  Data : correspond au type <T> passer à GetData
 -  DataRaw : la donnée brute récupérer au format Json de l'api
 -  M3Record : est un dictionnaire clé, valeur.

