using DataAccessLayer.Models;
using DataAccessLayer.Repository;
using System;
using System.Collections.Generic;

namespace BusinessLogicLayer.Service
{
	public class CategoryService
	{
		private readonly CategoryRepository _repo = new CategoryRepository();

		public void Add(Category c)
		{
			Validate(c);

			if (_repo.GetById(c.Id) != null)
				throw new Exception("Category Id already exists.");

			_repo.Add(c);
		}

		public List<Category> GetAll() => _repo.GetAll();

		public Category GetById(int id)
		{
			var c = _repo.GetById(id);  
			if (c == null)
				throw new Exception($"Category with id={id} not found");
			return c;
		}


		public void Update(Category c)
		{
			Validate(c);

			if (_repo.GetById(c.Id) == null)
				throw new Exception("Category not found.");

			_repo.Update(c);
		}

		public void Delete(int id) => _repo.Delete(id);

		public List<Category> Search(string keyword) => _repo.Search(keyword);

		private static void Validate(Category c)
		{
			if (c == null) throw new Exception("Category is null.");
			if (c.Id <= 0) throw new Exception("Category Id must be > 0.");
			if (string.IsNullOrWhiteSpace(c.Name)) throw new Exception("Category Name is required.");
		}
	}
}
