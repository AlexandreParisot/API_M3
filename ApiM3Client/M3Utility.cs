using ApiM3Connector.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace ApiM3Connector
{
    public static class M3Utility
    {


        #region champs perso


        public static async Task<bool> MajChampsPersos(object GP_Perso, string CFMG, string CFMF, string value)
        {
            //supprime l'enregistrement + le recrée

           M3Client.myRecord.Clear();
            M3Client. myRecord.Set("UDFT", "1");
            M3Client. myRecord.Set("ITNO", value);
            M3Client. myRecord.Set("CFMG", CFMG);
            M3Client.myRecord.Set("CFMF", CFMF);

            var result = await M3Client.SetDataAsync("CMS474MI", "DltUdefContent");
            if (!result.Success)
              return false;
           
            //pas de creation de champ perso si la valeur est null ou vide.
            if (string.IsNullOrEmpty(GP_Perso?.ToString()))
                return true;

             M3Client.myRecord.Clear();
             M3Client.myRecord.Set("UDFT", "1");
             M3Client.myRecord.Set("ITNO", value);
             M3Client.myRecord.Set("CFMG", CFMG);
             M3Client.myRecord.Set("CFMF", CFMF);
            M3Client.myRecord.Set("CFMA", GP_Perso);


            result = await M3Client.SetDataAsync("CMS474MI", "AddUdefContent");
            if (!result.Success)
               return false;
            
            return (bool)result.Data;


        }


        public static async Task<bool> DeleteChampsPersos(string GP_Perso, string CFMG, string CFMF, string value)
        {
            //supprime l'enregistrement + le recrée

             M3Client.myRecord.Clear();
             M3Client.myRecord.Set("UDFT", "1");
             M3Client.myRecord.Set("ITNO", value);
             M3Client.myRecord.Set("CFMG", CFMG);
            M3Client.myRecord.Set("CFMF", CFMF);

            var result = await M3Client.SetDataAsync("CMS474MI", "DltUdefContent");
            if (!result.Success)
               return false;
            
            return (bool)result.Data;
        }

        public static async Task<string> GetValueChampsPersos(string ITNO, string CFMG, string CFMF)
        {
            //pas de creation de champ perso si la valeur est null ou vide.
            if (string.IsNullOrEmpty(ITNO))
                throw new ArgumentNullException(nameof(ITNO));
            if (string.IsNullOrEmpty(CFMF))
                throw new ArgumentNullException(nameof(CFMF));
            if (string.IsNullOrEmpty(CFMG))
                throw new ArgumentNullException(nameof(CFMG));

             M3Client.myRecord.Clear();
             M3Client.myRecord.Set("UDFT", "1");
            M3Client.myRecord.Set("ITNO", ITNO);

            var result = await M3Client.GetDataAsync("CMS474MI", "LstUdefContent");
            if (!result.Success)
                return string.Empty;

            var Items = result.M3Records;
            foreach (var item in Items)
            {
                if (item.Keys.Contains("CFMF") && item.Keys.Contains("CFMG"))
                    if (item["CFMF"].ToString().Trim() == CFMF && item["CFMG"].ToString().Trim() == CFMG)
                    {
                        return item["CFMA"].ToString().Trim()??string.Empty;
                    }

            }

            return string.Empty;

        }


        #endregion


        #region gestion Cugex

        public static async Task<bool> MajCugex(string module, string pk01, string pk02 = "", string pk03 = "", string pk04 = "", string pk05 = "", Dictionary<string, string> fields = null)
        {


            M3Client.myRecord.Clear();

            M3Client.myRecord.Add("FILE", module);
            M3Client.myRecord.Add("PK01", pk01);
            if (!string.IsNullOrEmpty(pk02))
                M3Client.myRecord.Set("PK02", pk02);
            if (!string.IsNullOrEmpty(pk03))
                M3Client.myRecord.Set("PK03", pk03);
            if (!string.IsNullOrEmpty(pk04))
                M3Client.myRecord.Set("PK04", pk04);
            if (!string.IsNullOrEmpty(pk05))
                M3Client.myRecord.Set("PK05", pk05);

            if (fields != null && fields.Count > 0)
            {
                foreach (var field in fields)
                {
                    M3Client.myRecord.Add(field.Key, field.Value);
                }
            }

            M3Response result = await M3Client.SetDataAsync("CUSEXTMI", "DelFieldValue");
            if (result.Success)
                result = await M3Client.SetDataAsync("CUSEXTMI", "AddFieldValue");

            return result.Success;


        }


        public static async Task<bool> DeleteCugex(string module, string pk01, string pk02 = "", string pk03 = "", string pk04 = "", string pk05 = "", Dictionary<string, string> fields = null)
        {


            M3Client.myRecord.Clear();
            M3Client.myRecord.Add("FILE", module);
            M3Client.myRecord.Add("PK01", pk01);
            if (!string.IsNullOrEmpty(pk02))
                M3Client.myRecord.Add("PK02", pk02);
            if (!string.IsNullOrEmpty(pk03))
                M3Client.myRecord.Add("PK03", pk03);
            if (!string.IsNullOrEmpty(pk04))
                M3Client.myRecord.Add("PK04", pk04);
            if (!string.IsNullOrEmpty(pk05))
                M3Client.myRecord.Add("PK05", pk05);

            if (fields != null && fields.Count > 0)
            {
                foreach (var field in fields)
                {
                    M3Client.myRecord.Add(field.Key, field.Value);
                }
            }

            M3Response result = await M3Client.SetDataAsync("CUSEXTMI", "DelFieldValue");

            return result.Success;


        }

        #endregion

    }
}
