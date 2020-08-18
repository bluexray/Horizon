using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Horizon.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Horizon.Core.Data
{
    public class BaseDBConfig
    {
    //    public static (List<MutiDbOperate>, List<MutiDbOperate>) MutiConnectionString => MutiInitConn();

    //    private static string DifDBConnOfSecurity(params string[] conn)
    //    {
    //        foreach (var item in conn)
    //        {
    //            try
    //            {
    //                if (File.Exists(item))
    //                {
    //                    return File.ReadAllText(item).Trim();
    //                }
    //            }
    //            catch (System.Exception)
    //            {
    //            }
    //        }

    //        return conn[conn.Length - 1];
    //    }


    //    public static (List<MutiDbOperate>, List<MutiDbOperate>) MutiInitConn()
    //    {
    //        List<MutiDbOperate> listdatabase = new List<MutiDbOperate>();
    //        List<MutiDbOperate> listdatabaseSimpleDB = new List<MutiDbOperate>(); //单库
    //        List<MutiDbOperate> listdatabaseSlaveDB = new List<MutiDbOperate>(); //从库

    //        string Path = "appsettings.json";
    //        using (var file = new StreamReader(Path))
    //        using (var reader = new JsonTextReader(file))
    //        {
    //            var jObj = (JObject) JToken.ReadFrom(reader);
    //            if (!string.IsNullOrWhiteSpace("DBS"))
    //            {
    //                var secJt = jObj["DBS"];
    //                if (secJt != null)
    //                {
    //                    for (int i = 0; i < secJt.Count(); i++)
    //                    {
    //                        if (secJt[i]["Enabled"].ObjToBool())
    //                        {
    //                            listdatabase.Add(SpecialDbString(new MutiDbOperate()
    //                            {
    //                                ConnId = secJt[i]["ConnId"].ObjToString(),
    //                                Conn = secJt[i]["Connection"].ObjToString(),
    //                                DbType = (DataBaseType) (secJt[i]["DBType"].ObjToInt()),
    //                                HitRate = secJt[i]["HitRate"].ObjToInt(),
    //                            }));
    //                        }
    //                    }
    //                }
    //            }

    //            // 单库，且不开启读写分离，只保留一个
    //            if (!ConfigHelper.App(new string[] {"CQRSEnabled"}).ObjToBool() &&
    //                !ConfigHelper.App(new string[] {"MutiDBEnabled"}).ObjToBool())
    //            {
    //                if (listdatabase.Count == 1)
    //                {
    //                    return (listdatabase, listdatabaseSlaveDB);
    //                }
    //                else
    //                {
    //                    var dbFirst = listdatabase.FirstOrDefault(d =>
    //                        d.ConnId == ConfigHelper.App(new string[] {"MainDB"}).ObjToString());
    //                    if (dbFirst == null)
    //                    {
    //                        dbFirst = listdatabase.FirstOrDefault();
    //                    }

    //                    listdatabaseSimpleDB.Add(dbFirst);
    //                    return (listdatabaseSimpleDB, listdatabaseSlaveDB);
    //                }
    //            }


    //            // 读写分离，且必须是单库模式，获取从库
    //            if (ConfigHelper.App(new string[] {"CQRSEnabled"}).ObjToBool() &&
    //                !ConfigHelper.App(new string[] {"MutiDBEnabled"}).ObjToBool())
    //            {
    //                if (listdatabase.Count > 1)
    //                {
    //                    listdatabaseSlaveDB = listdatabase
    //                        .Where(d => d.ConnId != ConfigHelper.App(new string[] {"MainDB"}).ObjToString()).ToList();
    //                }
    //            }



    //            return (listdatabase, listdatabaseSlaveDB);
    //        }
    //    }
    //    private static MutiDbOperate SpecialDbString(MutiDbOperate mutiDBOperate)
    //    {
    //        if (mutiDBOperate.DbType == DataBaseType.Sqlite)
    //        {
    //            mutiDBOperate.Conn = $"DataSource=" + Path.Combine(Environment.CurrentDirectory, mutiDBOperate.Conn);
    //        }
    //        //else if (mutiDBOperate.DbType == DataBaseType.SqlServer)
    //        //{
    //        //    mutiDBOperate.Conn = DifDBConnOfSecurity(@"D:\my-file\dbCountPsw1.txt", @"c:\my-file\dbCountPsw1.txt", mutiDBOperate.Conn);
    //        //}
    //        else if (mutiDBOperate.DbType == DataBaseType.MySql)
    //        {
    //            mutiDBOperate.Conn = DifDBConnOfSecurity(@"D:\my-file\dbCountPsw1_MySqlConn.txt", @"c:\my-file\dbCountPsw1_MySqlConn.txt", mutiDBOperate.Conn);
    //        }
    //        else if (mutiDBOperate.DbType == DataBaseType.Oracle)
    //        {
    //            mutiDBOperate.Conn = DifDBConnOfSecurity(@"D:\my-file\dbCountPsw1_OracleConn.txt", @"c:\my-file\dbCountPsw1_OracleConn.txt", mutiDBOperate.Conn);
    //        }

    //        return mutiDBOperate;
    //    }
    }
}
