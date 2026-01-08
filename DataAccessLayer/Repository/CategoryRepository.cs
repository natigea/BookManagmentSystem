using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DataAccessLayer.Repository
{
	public class CategoryRepository : IRepository<Category>
	{
		private const int ID_LEN = 5;
		private const int NAME_LEN = 25;
		private const int DESC_LEN = 40;

		private readonly string _path;

		public CategoryRepository()
		{
			_path = ResolvePath("categories.txt");
		}

		public void Add(Category entity) =>
			File.AppendAllLines(_path, new[] { ToLine(entity) });

		public Category? GetById(int id)
		{
			return GetAll().FirstOrDefault(c => c.Id == id);
		}


		public List<Category> GetAll()
		{
			var lines = File.ReadAllLines(_path);
			var list = new List<Category>();

			foreach (var line in lines)
			{
				if (string.IsNullOrWhiteSpace(line)) continue;
				if (line.Length < ID_LEN + NAME_LEN + DESC_LEN) continue;

				list.Add(Parse(line));
			}

			return list;
		}

		public void Update(Category entity)
		{
			var all = GetAll();
			var idx = all.FindIndex(x => x.Id == entity.Id);
			if (idx < 0) throw new Exception("Category not found.");

			all[idx] = entity;
			File.WriteAllLines(_path, all.Select(ToLine));
		}

		public void Delete(int id)
		{
			var all = GetAll().Where(x => x.Id != id).Select(ToLine).ToList();
			File.WriteAllLines(_path, all);
		}

		public List<Category> Search(string keyword)
		{
			keyword ??= "";
			keyword = keyword.Trim().ToLower();

			return GetAll()
				.Where(c => (c.Name ?? "").ToLower().Contains(keyword))
				.ToList();
		}

		private static Category Parse(string line)
		{
			var id = int.Parse(line.Substring(0, ID_LEN));
			var name = line.Substring(ID_LEN, NAME_LEN).Trim();
			var desc = line.Substring(ID_LEN + NAME_LEN, DESC_LEN).Trim();

			return new Category { Id = id, Name = name, Description = desc };
		}

		private static string ToLine(Category c)
		{
			return c.Id.ToString().PadLeft(ID_LEN, '0')
				 + (c.Name ?? "").PadRight(NAME_LEN)
				 + (c.Description ?? "").PadRight(DESC_LEN);
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
