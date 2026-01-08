using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace DataAccessLayer.Repository
{
	public class MemberRepository : IRepository<Member>
	{
		private const int ID_LEN = 5;
		private const int NAME_LEN = 30;
		private const int EMAIL_LEN = 30;
		private const int PHONE_LEN = 15;
		private const int DATE_LEN = 8; 
		private const int ACTIVE_LEN = 1;

		private readonly string _path;

		public MemberRepository()
		{
			_path = ResolvePath("members.txt");
		}

		public void Add(Member entity) =>
			File.AppendAllLines(_path, new[] { ToLine(entity) });

		public Member? GetById(int id)
		{
			return GetAll().FirstOrDefault(c => c.Id == id);
		}


		public List<Member> GetAll()
		{
			var lines = File.ReadAllLines(_path);
			var list = new List<Member>();

			foreach (var line in lines)
			{
				if (string.IsNullOrWhiteSpace(line)) continue;
				if (line.Length < TotalLen()) continue;

				list.Add(Parse(line));
			}

			return list;
		}

		public void Update(Member entity)
		{
			var all = GetAll();
			var idx = all.FindIndex(x => x.Id == entity.Id);
			if (idx < 0) throw new Exception("Member not found.");

			all[idx] = entity;
			File.WriteAllLines(_path, all.Select(ToLine));
		}

		public void Delete(int id)
		{
			var all = GetAll().Where(x => x.Id != id).Select(ToLine).ToList();
			File.WriteAllLines(_path, all);
		}

		public List<Member> Search(string keyword)
		{
			keyword ??= "";
			keyword = keyword.Trim().ToLower();

			return GetAll()
				.Where(m =>
					(m.FullName ?? "").ToLower().Contains(keyword) ||
					(m.Email ?? "").ToLower().Contains(keyword))
				.ToList();
		}

		private static int TotalLen() =>
			ID_LEN + NAME_LEN + EMAIL_LEN + PHONE_LEN + DATE_LEN + ACTIVE_LEN;

		private static Member Parse(string line)
		{
			int p = 0;

			var id = int.Parse(line.Substring(p, ID_LEN)); p += ID_LEN;
			var name = line.Substring(p, NAME_LEN).Trim(); p += NAME_LEN;
			var email = line.Substring(p, EMAIL_LEN).Trim(); p += EMAIL_LEN;
			var phone = line.Substring(p, PHONE_LEN).Trim(); p += PHONE_LEN;

			var dateRaw = line.Substring(p, DATE_LEN); p += DATE_LEN;
			var date = DateTime.ParseExact(dateRaw, "yyyyMMdd", CultureInfo.InvariantCulture);

			var active = line.Substring(p, ACTIVE_LEN) == "1";

			return new Member
			{
				Id = id,
				FullName = name,
				Email = email,
				PhoneNumber = phone,
				MembershipDate = date,
				IsActive = active
			};
		}

		private static string ToLine(Member m)
		{
			var date = m.MembershipDate.ToString("yyyyMMdd");

			return m.Id.ToString().PadLeft(ID_LEN, '0')
				 + (m.FullName ?? "").PadRight(NAME_LEN)
				 + (m.Email ?? "").PadRight(EMAIL_LEN)
				 + (m.PhoneNumber ?? "").PadRight(PHONE_LEN)
				 + date.PadLeft(DATE_LEN, '0')
				 + (m.IsActive ? "1" : "0");
		}

		private static string ResolvePath(string fileName)
		{
			var current = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

			for (int i = 0; i < 8 && current != null; i++)
			{
				var candidateData = Path.Combine(current.FullName, "Data");
				if (Directory.Exists(candidateData))
				{
					var p = Path.Combine(candidateData, fileName);
					if (!File.Exists(p)) File.WriteAllText(p, string.Empty);
					return p;
				}

				current = current.Parent;
			}

			var fallbackData = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
			Directory.CreateDirectory(fallbackData);

			var fallbackPath = Path.Combine(fallbackData, fileName);
			if (!File.Exists(fallbackPath)) File.WriteAllText(fallbackPath, string.Empty);
			return fallbackPath;
		}
	}
}
