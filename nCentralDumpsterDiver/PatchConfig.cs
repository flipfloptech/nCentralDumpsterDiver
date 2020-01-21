using com.nable.agent.framework.Configuration;
using Serilog;
using Harmony;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog.Sinks.SystemConsole.Themes;
using System.Reflection;

namespace nCentralDumpsterDiver
{
	[HarmonyPatch(typeof(ConfigurationFile))]
	[HarmonyPatch("OpenFile")]
	[HarmonyPatch(new Type[] { typeof(string), typeof(bool) })]
	class HarmonyPatch_ConfigurationFile_OpenFile
    {

		[HarmonyPrefix]
		static bool Patch_Prefix(ref string path, ref bool bWrite, ref FileStream __result)
		{
			return false;
		}
	}
	[HarmonyPatch(typeof(ConfigurationFile))]
	[HarmonyPatch("backup")]
	[HarmonyPatch(new Type[] { typeof(string) })]
	class HarmonyPatch_ConfigurationFile_backup
	{
		[HarmonyPrefix]
		static bool Patch_Prefix(ref string dir)
		{
			return false;
		}
	}
	[HarmonyPatch(typeof(ConfigurationFile))]
	[HarmonyPatch("save")]
	[HarmonyPatch(new Type[] { typeof(string) })]
	class HarmonyPatch_ConfigurationFile_save
	{
		[HarmonyPrefix]
		static bool Patch_Prefix(ref string dir)
		{
			return false;
		}
	}
	[HarmonyPatch(typeof(ConfigurationFile))]
	[HarmonyPatch("WriteFile")]
	[HarmonyPatch(new Type[] { typeof(string), typeof(byte[]) })]
	class HarmonyPatch_ConfigurationFile_WriteFile
	{
		[HarmonyPrefix]
		static bool Patch_Prefix(ref string path, ref byte[] data)
		{
			return false;
		}
	}
	[HarmonyPatch(typeof(ConfigurationFile))]
	[HarmonyPatch("ReadFile")]
	[HarmonyPatch(new Type[] { typeof(string) })]
	class HarmonyPatch_ConfigurationFile_ReadFile
	{
		[HarmonyPrefix]
		static bool Patch_Prefix(ref string path, ref string __result)
		{
			__result = "";
			return false;
		}
	}

	[HarmonyPatch(typeof(Directory))]
	[HarmonyPatch("CreateDirectory")]
	[HarmonyPatch(new Type[] { typeof(string) })]
	class HarmonyPatch_Directory_CreateDirectory
	{
		[HarmonyPrefix]
		static bool Patch_Prefix(ref string path, ref DirectoryInfo __result)
		{
			if (path.EndsWith("\\config"))
			{
				__result = null;
				return false;
			}
			else
			{
				return true;
			}
		}
	}
}