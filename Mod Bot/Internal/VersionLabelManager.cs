using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using ModLibrary;

namespace InternalModBot
{
	/// <summary>
	/// Takes care of editing the version label
	/// </summary>
	public class VersionLabelManager : Singleton<VersionLabelManager>
	{
		/// <summary>
		/// The version label itself
		/// </summary>
		public Text VersionLabel;

		/// <summary>
		/// Sets the provided line of the version label to the provided text, creating new lines if neccicary
		/// </summary>
		/// <param name="line"></param>
		/// <param name="value"></param>
		public void SetLine(int line, string value)
		{
			if (VersionLabel == null)
			{
				VersionLabel = GameUIRoot.Instance.TitleScreenUI.VersionLabel;
				VersionLabel.horizontalOverflow = UnityEngine.HorizontalWrapMode.Overflow;
			}
				

			string[] lines = VersionLabel.text.Split("\n".ToCharArray());

			if (lines.Length > line)
			{
				lines[line] = value;
			} else
			{
				int oldLength = lines.Length;
				string[] newLines = new string[line + 1];
				Array.Copy(lines, newLines, lines.Length);
				for (int i = oldLength; i < newLines.Length; i++)
				{
					newLines[i] = "";
				}

				newLines[line] = value;

				lines = newLines;
			}

			string joinedLines = lines.Join("\n");
			VersionLabel.text = joinedLines;
		}

	}
}
