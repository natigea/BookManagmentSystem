using System;
using System.Collections.Generic;
using DataAccessLayer.Models;
using DataAccessLayer.Repository;

namespace BusinessLogicLayer.Service
{
	public class BookService
	{
		private readonly BookRepository _bookRepo = new BookRepository();
		private readonly CategoryRepository _catRepo = new CategoryRepository(); 

		public void Add(Book book)
		{
			var cat = _catRepo.GetById(book.CategoryId);
			if (cat == null)
				throw new Exception($"Category with id={book.CategoryId} not found");

			var existing = _bookRepo.GetById(book.Id);
			if (existing != null)
				throw new Exception($"Book with id={book.Id} already exists");

			_bookRepo.Add(book);
		}

		public List<Book> GetAll() => _bookRepo.GetAll();

		public Book GetById(int id)
		{
			var b = _bookRepo.GetById(id);
			if (b == null)
				throw new Exception($"Book with id={id} not found");
			return b;
		}

		public void Update(Book b)
		{
			var existing = _bookRepo.GetById(b.Id);
			if (existing == null)
				throw new Exception($"Book with id={b.Id} not found");

			var cat = _catRepo.GetById(b.CategoryId);
			if (cat == null)
				throw new Exception($"Category with id={b.CategoryId} not found");

			_bookRepo.Update(b);
		}

		public void Delete(int id)
		{
			var existing = _bookRepo.GetById(id);
			if (existing == null)
				throw new Exception($"Book with id={id} not found");

			_bookRepo.Delete(id);
		}

		public List<Book> Search(string keyword)
		{
			keyword ??= "";
			keyword = keyword.Trim();

			var all = _bookRepo.GetAll();
			var result = new List<Book>();

			foreach (var b in all)
			{
				if ((b.Title ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
					(b.Author ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
					b.CategoryId.ToString().Contains(keyword, StringComparison.OrdinalIgnoreCase))
				{
					result.Add(b);
				}
			}

			return result;
		}
	}
}
