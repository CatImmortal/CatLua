using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.IO;

namespace CatLua
{
	public class CallLuac
	{
		/// <summary>
		/// 编译LuaScripts下所有Lua代码，并将编译后的字节码输出到Resources目录下
		/// </summary>
		[MenuItem("CatLua/编译Lua代码为字节码")]
		public static void CmdCallLuac()
		{
			foreach (string item in Directory.GetFiles(Application.dataPath + "/Resources"))
			{
				File.Delete(item);
			}

			string[] files = Directory.GetFiles(Application.dataPath + "/Scripts/LuaScripts", "*.lua");
			foreach (string item in files)
			{
				string target = item.Replace("Assets/Scripts/LuaScripts", "Assets/Resources");
				target = target.Replace(".lua", ".bytes");
				target = target.Replace('/', '\\');
				//Debug.Log(target);
				ProcessCommand("luac", " -o " + target + " " + item);
			}

			AssetDatabase.Refresh();
		}


		public static void ProcessCommand(string command, string argument)
		{
			ProcessStartInfo info = new ProcessStartInfo(command);
			info.Arguments = argument;
			info.CreateNoWindow = false;
			info.ErrorDialog = true;
			info.UseShellExecute = true;

			if (info.UseShellExecute)
			{
				info.RedirectStandardOutput = false;
				info.RedirectStandardError = false;
				info.RedirectStandardInput = false;
			}
			else
			{
				info.RedirectStandardOutput = true;
				info.RedirectStandardError = true;
				info.RedirectStandardInput = true;
				info.StandardOutputEncoding = System.Text.Encoding.UTF8;
				info.StandardErrorEncoding = System.Text.Encoding.UTF8;
			}

			Process process = Process.Start(info);

			if (!info.UseShellExecute)
			{
				Debug.Log(process.StandardOutput);
				Debug.Log(process.StandardError);
			}

			process.WaitForExit();
			process.Close();
		}

	}
}

