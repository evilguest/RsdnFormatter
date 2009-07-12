using System.Text.RegularExpressions;

using Rsdn.Framework.Common;

namespace Rsdn.Framework.Formatting
{
	public static partial class Format
	{
		/// <summary>
		/// ������������� ������� �������������� ��������� ������.
		/// </summary>
		public class Forum : RsdnMbrObject
		{
			/// <summary>
			/// Regex for detecting Re: and Re[number]: prefixes in subject at the start of the line
			/// </summary>
			private static readonly Regex reDetecter =
				new Regex(@"(?in)^((Re|��)(\[(?<number>\d+)\])?:\s*)+", RegexOptions.Compiled);

			/// <summary>
			/// ������� ���� '����' ��� ��������������.
			/// </summary>
			/// <param name="subj">�������� ����.</param>
			/// <returns>���������.</returns>
			public static string GetEditSubject(string subj)
			{
				if (string.IsNullOrEmpty(subj))
					return subj;

				// ������� ������� Re[..]
				return reDetecter.Replace(subj, "");
			}

			/// <summary>
			/// ������������� ���� ���������.
			/// </summary>
			/// <remarks>
			/// ��������� ������� '<b>Re</b>' � ����� ���� ���������.
			/// ��� ������� ������ � ����� ����������� ������� '<b>Re:</b>', 
			/// ��� ���� ����������� '<b>Re[n]:</b>', ��� <b>n</b> - ������� �����������.
			/// </remarks>
			/// <param name="oldSubject">���� ����������� ���������.</param>
			/// <param name="newSubject">���� ������ ���������.</param>
			/// <returns>���������� ��������� � ����������� ��������� 'Re'.</returns>
			public static string AdjustSubject(string oldSubject, string newSubject)
			{
				return GetRePrefix(GetSubjectDeep(oldSubject) + 1) + newSubject;
			}

			/// <summary>
			/// ���������� ������� ����������� ���� ���������.
			/// </summary>
			/// <param name="subject">���� ���������</param>
			/// <returns>������� �����������.</returns>
			public static int GetSubjectDeep(string subject)
			{
				var level = 0;
				var reMatch = reDetecter.Match(subject);
				if (reMatch.Success)
				{
					level = !reMatch.Groups["number"].Success ? 1 :
						ToInt(reMatch.Groups["number"].Captures[0].Value);
				}
				return level;
			}

			/// <summary>
			/// ���������� ������� ���� ���������.
			/// </summary>
			/// <remarks>
			/// ��� ������� ������ � ����� ������� '<b>Re:</b>', 
			/// ��� ���� ����������� '<b>Re[n]:</b>', ��� <b>n</b> - ������� �����������.
			/// </remarks>
			/// <param name="level">������� ����������� ���������.</param>
			/// <returns>�������.</returns>
			private static string GetRePrefix(int level)
			{
				return (level == 1) ? "Re: " : string.Format("Re[{0}]: ", level);
			}

			/// <summary>
			/// ������������� ���� ���������.
			/// </summary>
			/// <remarks>
			/// ������������ ������� ����������� ��������� (Re[xxx])
			/// ������������ ������ �����.
			/// </remarks>
			/// <param name="level">������� ����������� ��������� ���������.</param>
			/// <param name="subject">���� ���������.</param>
			/// <returns>������������������ ���� ���������.</returns>
			public static string AdjustSubject(int level, string subject)
			{
				return reDetecter.Replace(subject, new MatchEvaluator(
					match =>
						GetRePrefix(match.Groups["number"].Success ?
							int.Parse(match.Groups["number"].Captures[0].Value) - level : 1)
					));
			}

			/// <summary>
			/// ���������� ���������� ��� ��������� ����.
			/// </summary>
			/// <param name="nick">���.</param>
			/// <returns>����������.</returns>
			public static string GetShortNick(string nick)
			{
				// �������� ���������� ��� ����.
				//
				var shortname = "";

				if (nick.Length <= 3 && !nick.Contains(" "))
				{
					// ��� ������ ��� ��������.
					shortname = nick
						.Replace("&", "")
						.Replace("<", "")
						.Replace(">", "")
						.Replace("\"", "")
						.Replace("'", "");
				}
				else if (nick == "Igor Trofimov")
				{
					shortname = "iT";
				}
				else if (nick == "_MarlboroMan_")
				{
					shortname = "_MM_";
				}
				else if (nick == "Hacker_Delphi")
				{
					shortname = "H_D";
				}
				else
				{
					// �������� ��� ����� ������� �� �������.
					var un = Regex.Replace(nick, @"\W+", " ").Trim();

					// ������� ������� ������� �������� � �����.
					if (!un.Contains(" "))
					{
						shortname = Regex.Replace(un, @"[a-z�-�0-9]", "");
						if (shortname.Length > 3)
							shortname = shortname.Substring(0, 3);
					}

					// ���� ��� ���� � ������ ��������.
					if (shortname.Length == 0)
					{
						// ����������� �� ������ ����.
						var sa = un.Split(' ');

						// ���� ������ ������ �������.
						for (var i = 0; i < sa.Length && i < 3; i++)
							if (sa[i].Length > 0)
								shortname += sa[i][0];

						// �������� � �������� ��������.
						shortname = shortname.ToUpper();
					}
				}

				return shortname;
			}

			/// <summary>
			/// ������� ��������� � �����������.
			/// </summary>
			/// <param name="msg">���������.</param>
			/// <param name="nick">����� ���������.</param>
			/// <returns>������������ ���������</returns>
			public static string GetEditMessage(string msg, string nick)
			{
				return GetEditMessage(msg, nick, false);
			}

			/// <summary>
			/// ������� ��������� � �����������.
			/// </summary>
			/// <param name="msg">���������.</param>
			/// <param name="nick">����� ���������.</param>
			/// <param name="moderator">== true, ���� ������������ - ���������.</param>
			/// <returns>������������ ���������</returns>
			public static string GetEditMessage(string msg, string nick, bool moderator)
			{
				// �������� ���������� ��� ����.
				//
				string shortname = GetShortNick(nick);

				// ���� ������������ �� ���������. ��� [moderator] ���������.
				// �������������� �������� ������������� ��� ���������� ���������.
				//
				if (moderator == false)
				{
					msg = TextFormatter.RemoveModeratorTag(msg);
				}
				// ������� �������
				//
				msg = TextFormatter.RemoveTaglineTag(msg);

				// ������������ ������.
				//
				msg = Regex.Replace(msg, @"^\s*[-\w\.]{0,5}>+", "$&>", RegexOptions.Multiline);
				msg = Regex.Replace(msg, @"(?m)^(?!\s*[-\w\.]{0,5}>|\s*$)", shortname + ">");
				msg = "������������, " + nick + ", �� ������:\n\n" + msg;

				return msg;
			}


		}
	}
}
