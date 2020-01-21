using com.nable.agent.framework.AssetDiscovery;
using com.nable.agent.framework.serverproxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nCentralDumpsterDiver
{
    class HashGenerator
    {
		public static List<Pair<int, bool>> GetKeyColumnIndexes(string[] columns, CimKey[] keyColumns)
		{
			List<Pair<int, bool>> list = new List<Pair<int, bool>>();
			foreach (CimKey cimKey in keyColumns)
			{
				for (int j = 0; j < columns.Length; j++)
				{
					if (columns[j].Equals(cimKey.Key, StringComparison.Ordinal))
					{
						list.Add(new Pair<int, bool>(j, cimKey.CaseSensitive));
						break;
					}
				}
			}
			return list;
		}
		public static void ComputeHash(T_CimData cimData)
		{
			if (cimData != null)
			{
				uint theStartingCRC = 0u;
				if (cimData.CimRecords != null)
				{
					foreach (T_CimRecord t_CimRecord in cimData.CimRecords)
					{
						if (t_CimRecord.ContentHash != null)
						{
							theStartingCRC = CRC32.Crc32(theStartingCRC, t_CimRecord.ContentHash);
						}
					}
				}
				if (cimData.CimTableName != null)
				{
					theStartingCRC = CRC32.Crc32(theStartingCRC, cimData.CimTableName);
				}
				cimData.ContentHash = theStartingCRC.ToString("x2");
			}
		}

		// Token: 0x06000207 RID: 519 RVA: 0x0000AEEC File Offset: 0x000090EC
		public static void ComputeHash(T_CimRecord cimRecord, List<Pair<int, bool>> keyColumnIndexes)
		{
			if (cimRecord != null)
			{
				uint theStartingCRC = 0u;
				uint theStartingCRC2 = 0u;
				if (cimRecord.CimValueFields != null)
				{
					int index;
					for (index = 0; index < cimRecord.CimValueFields.Length; index++)
					{
						string text = cimRecord.CimValueFields[index];
						if (text != null)
						{
							theStartingCRC2 = CRC32.Crc32(theStartingCRC2, text);
							Pair<int, bool> pair = keyColumnIndexes.Find((Pair<int, bool> x) => x.Item1 == index);
							if (pair != null)
							{
								theStartingCRC = CRC32.Crc32(theStartingCRC, pair.Item2 ? text : text.ToLower());
							}
						}
					}
				}
				cimRecord.Recordid = theStartingCRC.ToString("x2");
				cimRecord.ContentHash = theStartingCRC2.ToString("x2");
			}
		}

		// Token: 0x06000208 RID: 520 RVA: 0x0000AFB8 File Offset: 0x000091B8
		public static string ComputeHash(string input)
		{
			return input.GetHashCode().ToString("X2");
		}
	}
}
