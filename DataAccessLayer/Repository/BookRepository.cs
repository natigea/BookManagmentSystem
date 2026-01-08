using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DataAccessLayer.Repository
{
	public class BookRepository : IRepository<Book>
	{

		private const int ID_LEN = 5;
		private const int TITLE_LEN = 30;
		private const int AUTHOR_LEN = 25;
		private const int ISBN_LEN = 13;
		private const int YEAR_LEN = 4;
		private const int CATID_LEN = 5;
		private const int AVAIL_LEN = 1;

		private readonly string _path;

		public BookRepository()
		{
			_path = ResolvePath("books.txt");
		}

		public void Add(Book entity)
		{
			File.AppendAllLines(_path, new[] { ToLine(entity) });
		}

		public Book? GetById(int id)
		{
			return GetAll().FirstOrDefault(b => b.Id == id);
		}


		public List<Book> GetAll()
		{
			var lines = File.ReadAllLines(_path);
			var list = new List<Book>();

			foreach (var line in lines)
			{
				if (string.IsNullOrWhiteSpace(line)) continue;
				if (line.Length < TotalLen()) continue;

				list.Add(Parse(line));
			}

			return list;
		}

		public void Update(Book entity)
		{
			var all = GetAll();
			var idx = all.FindIndex(x => x.Id == entity.Id);
			if (idx < 0) throw new Exception("Book not found.");

			all[idx] = entity;
			File.WriteAllLines(_path, all.Select(ToLine));
		}

		public void Delete(int id)
		{
			var all = GetAll().Where(x => x.Id != id).Select(ToLine).ToList();
			File.WriteAllLines(_path, all);
		}

		public List<Book> Search(string keyword)
		{
			keyword ??= "";
			keyword = keyword.Trim().ToLower();

			return GetAll()
				.Where(b =>
					(b.Title ?? "").ToLower().Contains(keyword) ||
					(b.Author ?? "").ToLower().Contains(keyword) ||
					(b.ISBN ?? "").ToLower().Contains(keyword))
				.ToList();
		}

		private static int TotalLen() =>
			ID_LEN + TITLE_LEN + AUTHOR_LEN + ISBN_LEN + YEAR_LEN + CATID_LEN + AVAIL_LEN;

		private static Book Parse(string line)
		{
			int p = 0;

			var id = int.Parse(line.Substring(p, ID_LEN)); p += ID_LEN;
			var title = line.Substring(p, TITLE_LEN).Trim(); p += TITLE_LEN;
			var author = line.Substring(p, AUTHOR_LEN).Trim(); p += AUTHOR_LEN;
			var isbn = line.Substring(p, ISBN_LEN).Trim(); p += ISBN_LEN;
			var year = int.Parse(line.Substring(p, YEAR_LEN)); p += YEAR_LEN;
			var catId = int.Parse(line.Substring(p, CATID_LEN)); p += CATID_LEN;
			var avail = line.Substring(p, AVAIL_LEN) == "1";

			return new Book
			{
				Id = id,
				Title = title,
				Author = author,
				ISBN = isbn,
				PublishedYear = year,
				CategoryId = catId,
				IsAvailable = avail
			};
		}

		private static string ToLine(Book b)
		{
			return b.Id.ToString().PadLeft(ID_LEN, '0')
				 + (b.Title ?? "").PadRight(TITLE_LEN)
				 + (b.Author ?? "").PadRight(AUTHOR_LEN)
				 + (b.ISBN ?? "").PadRight(ISBN_LEN)
				 + b.PublishedYear.ToString().PadLeft(YEAR_LEN, '0')
				 + b.CategoryId.ToString().PadLeft(CATID_LEN, '0')
				 + (b.IsAvailable ? "1" : "0");
		}

		private static string ResolvePath(string fileName)
		{
			var dataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
			Directory.CreateDirectory(dataFolder);

			var fullPath = Path.Combine(dataFolder, fileName);
			if (!File.Exists(fullPath)) File.WriteAllText(fullPath, string.Empty);

			return fullPath;
		}
	}
}
