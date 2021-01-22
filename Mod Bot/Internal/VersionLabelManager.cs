using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using ModLibrary;

namespace InternalModBot
{
	public class VersionLabelManager : Singleton<VersionLabelManager>
	{
		public Text Text;

		public void SetLine(int line, string value)
		{
			if (Text == null)
			{
				Text = GameUIRoot.Instance.TitleScreenUI.VersionLabel;
				Text.horizontalOverflow = UnityEngine.HorizontalWrapMode.Overflow;
			}
				

			string[] lines = Text.text.Split("\n".ToCharArray());

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
			Text.text = joinedLines;
		}

	}
}
