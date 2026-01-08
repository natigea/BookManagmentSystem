using BusinessLogicLayer.Service;
using DataAccessLayer.Models;
using System;
using System.Text;

namespace PresentationalLayer
{
	internal class Program
	{
		private static readonly BookService bookService = new BookService();
		private static readonly CategoryService categoryService = new CategoryService();
		private static readonly MemberService memberService = new MemberService();

		static void Main()
		{
			Console.OutputEncoding = Encoding.UTF8;
			Console.Title = "Library Control System";

			while (true)
			{
				DrawHeader("📚 Library Control System", "Main Menu");

				var choice = MenuSelect(new (string Key, string Text)[]
				{
					("1", "📘 Books"),
					("2", "🏷️ Categories"),
					("3", "👤 Members"),
					("0", "🚪 Exit")
				});

				try
				{
					switch (choice)
					{
						case "1": BooksMenu(); break;
						case "2": CategoriesMenu(); break;
						case "3": MembersMenu(); break;
						case "0": return;
						default:
							ToastError("Invalid choice.");
							break;
					}
				}
				catch (Exception ex)
				{
					ToastError($"ERROR: {ex.Message}");
				}
			}
		}

		//  BOOKS
		private static void BooksMenu()
		{
			while (true)
			{
				DrawHeader("📘 Books", "Book Management");

				var key = MenuSelect(new (string Key, string Text)[]
				{
					("1", "➕ Add"),
					("2", "📋 List all"),
					("3", "🔎 Get by Id"),
					("4", "✏️ Update"),
					("5", "🗑️ Delete"),
					("6", "🔍 Search (title/author/category)"),
					("0", "↩️ Back")
				});

				if (key == "0") return;

				switch (key)
				{
					case "1": AddBook(); break;
					case "2": ListBooks(); break;
					case "3": GetBookById(); break;
					case "4": UpdateBook(); break;
					case "5": DeleteBook(); break;
					case "6": SearchBooks(); break;
					default: ToastError("Invalid choice."); break;
				}
			}
		}

		private static void AddBook()
		{
			DrawHeader("➕ Add Book", "Enter data");

			ShowCategoriesMini(); 

			var b = new Book
			{
				Id = ReadInt("Id"),
				Title = ReadStr("Title"),
				Author = ReadStr("Author"),
				ISBN = ReadStr("ISBN"),
				PublishedYear = ReadInt("PublishedYear"),
				CategoryId = ReadInt("CategoryId"),
				IsAvailable = true
			};

			bookService.Add(b);
			ToastOk("Book added.");
		}

		private static void ListBooks()
		{
			DrawHeader("📋 Book List", "All books");

			var all = bookService.GetAll();
			if (all.Count == 0) { ToastInfo("List is empty."); return; }

			TableHeader("Id", "Title", "Author", "ISBN", "Year", "CatId", "Avail");
			foreach (var b in all)
			{
				TableRow(
					b.Id.ToString(),
					Trunc(b.Title, 22),
					Trunc(b.Author, 18),
					Trunc(b.ISBN, 13),
					b.PublishedYear.ToString(),
					b.CategoryId.ToString(),
					b.IsAvailable ? "✅" : "❌"
				);
			}

			Pause();
		}

		private static void GetBookById()
		{
			DrawHeader("🔎 Get Book", "Search by Id");

			var id = ReadInt("Id");
			var b = bookService.GetById(id);

			if (b == null) { ToastInfo("Not found."); return; }

			BoxInfo(
				$"📘 {b.Title}\n" +
				$"👤 Author: {b.Author}\n" +
				$"🔢 ISBN: {b.ISBN}\n" +
				$"📅 Year: {b.PublishedYear}\n" +
				$"🏷️ CategoryId: {b.CategoryId}\n" +
				$"📦 Available: {(b.IsAvailable ? "Yes ✅" : "No ❌")}"
			);
			Pause();
		}

		private static void UpdateBook()
		{
			DrawHeader("✏️ Update Book", "Enter new data");

			ShowCategoriesMini();

			var b = new Book
			{
				Id = ReadInt("Id (existing)"),
				Title = ReadStr("Title"),
				Author = ReadStr("Author"),
				ISBN = ReadStr("ISBN"),
				PublishedYear = ReadInt("PublishedYear"),
				CategoryId = ReadInt("CategoryId"),
				IsAvailable = ReadBool("IsAvailable (1/0)")
			};

			bookService.Update(b);
			ToastOk("Book updated.");
		}

		private static void DeleteBook()
		{
			DrawHeader("🗑️ Delete Book", "Delete by Id");

			var id = ReadInt("Id");
			if (!Confirm($"Delete book with Id={id}?")) { ToastInfo("Canceled."); return; }

			bookService.Delete(id);
			ToastOk("Book deleted.");
		}

		private static void SearchBooks()
		{
			DrawHeader("🔍 Search Books", "Search");

			var k = ReadStr("Keyword");
			var res = bookService.Search(k);

			if (res.Count == 0) { ToastInfo("No matches found."); return; }

			TableHeader("Id", "Title", "Author", "CatId");
			foreach (var b in res)
				TableRow(b.Id.ToString(), Trunc(b.Title, 28), Trunc(b.Author, 22), b.CategoryId.ToString());

			Pause();
		}

		//  CATEGORIES 
		private static void CategoriesMenu()
		{
			while (true)
			{
				DrawHeader("🏷️ Categories", "Category Management");

				var key = MenuSelect(new (string Key, string Text)[]
				{
					("1", "➕ Add"),
					("2", "📋 List all"),
					("3", "🔎 Get by Id"),
					("4", "✏️ Update"),
					("5", "🗑️ Delete"),
					("6", "🔍 Search (name)"),
					("0", "↩️ Back")
				});

				if (key == "0") return;

				switch (key)
				{
					case "1": AddCategory(); break;
					case "2": ListCategories(); break;
					case "3": GetCategoryById(); break;
					case "4": UpdateCategory(); break;
					case "5": DeleteCategory(); break;
					case "6": SearchCategories(); break;
					default: ToastError("Invalid choice."); break;
				}
			}
		}

		private static void AddCategory()
		{
			DrawHeader("➕ Add Category", "Enter data");

			var c = new Category
			{
				Id = ReadInt("Id"),
				Name = ReadStr("Name"),
				Description = ReadStr("Description")
			};

			categoryService.Add(c);
			ToastOk("Category added.");
		}

		private static void ListCategories()
		{
			DrawHeader("📋 Category List", "All categories");

			var all = categoryService.GetAll();
			if (all.Count == 0) { ToastInfo("List is empty."); return; }

			TableHeader("Id", "Name", "Description");
			foreach (var c in all)
				TableRow(c.Id.ToString(), Trunc(c.Name, 25), Trunc(c.Description, 35));

			Pause();
		}

		private static void GetCategoryById()
		{
			DrawHeader("🔎 Get Category", "Search by Id");

			var id = ReadInt("Id");
			var c = categoryService.GetById(id);

			if (c == null) { ToastInfo("Not found."); return; }

			BoxInfo($"🏷️ {c.Name}\n📝 {c.Description}");
			Pause();
		}

		private static void UpdateCategory()
		{
			DrawHeader("✏️ Update Category", "Enter new data");

			var c = new Category
			{
				Id = ReadInt("Id (existing)"),
				Name = ReadStr("Name"),
				Description = ReadStr("Description")
			};

			categoryService.Update(c);
			ToastOk("Category updated.");
		}

		private static void DeleteCategory()
		{
			DrawHeader("🗑️ Delete Category", "Delete by Id");

			var id = ReadInt("Id");
			if (!Confirm($"Delete category with Id={id}?")) { ToastInfo("Canceled."); return; }

			categoryService.Delete(id);
			ToastOk("Category deleted.");
		}

		private static void SearchCategories()
		{
			DrawHeader("🔍 Search Categories", "Search");

			var k = ReadStr("Keyword");
			var res = categoryService.Search(k);

			if (res.Count == 0) { ToastInfo("No matches found."); return; }

			TableHeader("Id", "Name");
			foreach (var c in res)
				TableRow(c.Id.ToString(), Trunc(c.Name, 35));

			Pause();
		}

		// MEMBERS 
		private static void MembersMenu()
		{
			while (true)
			{
				DrawHeader("👤 Members", "Member Management");

				var key = MenuSelect(new (string Key, string Text)[]
				{
					("1", "➕ Add"),
					("2", "📋 List all"),
					("3", "🔎 Get by Id"),
					("4", "✏️ Update"),
					("5", "🗑️ Delete"),
					("6", "🔍 Search (name/email)"),
					("0", "↩️ Back")
				});

				if (key == "0") return;

				switch (key)
				{
					case "1": AddMember(); break;
					case "2": ListMembers(); break;
					case "3": GetMemberById(); break;
					case "4": UpdateMember(); break;
					case "5": DeleteMember(); break;
					case "6": SearchMembers(); break;
					default: ToastError("Invalid choice."); break;
				}
			}
		}

		private static void AddMember()
		{
			DrawHeader("➕ Add Member", "Enter data");

			var m = new Member
			{
				Id = ReadInt("Id"),
				FullName = ReadStr("FullName"),
				Email = ReadStr("Email"),
				PhoneNumber = ReadStr("PhoneNumber"),
				MembershipDate = DateTime.Today,
				IsActive = true
			};

			memberService.Add(m);
			ToastOk("Member added.");
		}

		private static void ListMembers()
		{
			DrawHeader("📋 Member List", "All members");

			var all = memberService.GetAll();
			if (all.Count == 0) { ToastInfo("List is empty."); return; }

			TableHeader("Id", "FullName", "Email", "Phone", "Date", "Active");
			foreach (var m in all)
			{
				TableRow(
					m.Id.ToString(),
					Trunc(m.FullName, 22),
					Trunc(m.Email, 24),
					Trunc(m.PhoneNumber, 14),
					m.MembershipDate.ToString("yyyy-MM-dd"),
					m.IsActive ? "✅" : "❌"
				);
			}

			Pause();
		}

		private static void GetMemberById()
		{
			DrawHeader("🔎 Get Member", "Search by Id");

			var id = ReadInt("Id");
			var m = memberService.GetById(id);

			if (m == null) { ToastInfo("Not found."); return; }

			BoxInfo(
				$"👤 {m.FullName}\n" +
				$"✉️ Email: {m.Email}\n" +
				$"📞 Phone: {m.PhoneNumber}\n" +
				$"📅 Date: {m.MembershipDate:yyyy-MM-dd}\n" +
				$"🔘 Active: {(m.IsActive ? "Yes ✅" : "No ❌")}"
			);
			Pause();
		}

		private static void UpdateMember()
		{
			DrawHeader("✏️ Update Member", "Enter new data");

			var m = new Member
			{
				Id = ReadInt("Id (existing)"),
				FullName = ReadStr("FullName"),
				Email = ReadStr("Email"),
				PhoneNumber = ReadStr("PhoneNumber"),
				MembershipDate = DateTime.Today,
				IsActive = ReadBool("IsActive (1/0)")
			};

			memberService.Update(m);
			ToastOk("Member updated.");
		}

		private static void DeleteMember()
		{
			DrawHeader("🗑️ Delete Member", "Delete by Id");

			var id = ReadInt("Id");
			if (!Confirm($"Delete member with Id={id}?")) { ToastInfo("Canceled."); return; }

			memberService.Delete(id);
			ToastOk("Member deleted.");
		}

		private static void SearchMembers()
		{
			DrawHeader("🔍 Search Members", "Search");

			var k = ReadStr("Keyword");
			var res = memberService.Search(k);

			if (res.Count == 0) { ToastInfo("No matches found."); return; }

			TableHeader("Id", "FullName", "Email");
			foreach (var m in res)
				TableRow(m.Id.ToString(), Trunc(m.FullName, 26), Trunc(m.Email, 30));

			Pause();
		}

		// DESIGN

		private static void DrawHeader(string title, string subtitle)
		{
			Console.Clear();

			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine("╔══════════════════════════════════════════════════════════════════╗");

			Console.Write("║ ");
			Console.ForegroundColor = ConsoleColor.White;
			Console.Write(PadRightSafe(title, 64));
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine(" ║");

			Console.Write("║ ");
			Console.ForegroundColor = ConsoleColor.DarkGray;
			Console.Write(PadRightSafe(subtitle, 64));
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine(" ║");

			Console.WriteLine("╚══════════════════════════════════════════════════════════════════╝");
			Console.ResetColor();
			Console.WriteLine();
		}

		private static string MenuSelect((string Key, string Text)[] items)
		{
			Console.ForegroundColor = ConsoleColor.Gray;
			foreach (var it in items) Console.WriteLine($"  [{it.Key}] {it.Text}");
			Console.ResetColor();

			Console.WriteLine();
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.Write("➜ Select: ");
			Console.ResetColor();

			return (Console.ReadLine() ?? "").Trim();
		}

		private static void ToastOk(string message)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"✅ {message}");
			Console.ResetColor();
			Pause();
		}

		private static void ToastInfo(string message)
		{
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine($"ℹ️ {message}");
			Console.ResetColor();
			Pause();
		}

		private static void ToastError(string message)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine($"⛔ {message}");
			Console.ResetColor();
			Pause();
		}

		private static void BoxInfo(string text)
		{
			Console.ForegroundColor = ConsoleColor.DarkCyan;
			Console.WriteLine("┌──────────────────────────────────────────────────────────────────┐");
			Console.ResetColor();

			foreach (var line in (text ?? "").Split('\n'))
			{
				Console.Write("│ ");
				Console.ForegroundColor = ConsoleColor.White;
				Console.Write(PadRightSafe(line.TrimEnd('\r'), 64));
				Console.ResetColor();
				Console.WriteLine(" │");
			}

			Console.ForegroundColor = ConsoleColor.DarkCyan;
			Console.WriteLine("└──────────────────────────────────────────────────────────────────┘");
			Console.ResetColor();
		}

		private static void TableHeader(params string[] cols)
		{
			Console.ForegroundColor = ConsoleColor.DarkGray;
			Console.WriteLine(new string('─', 74));
			Console.ResetColor();

			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine(string.Join(" | ", cols));
			Console.ResetColor();

			Console.ForegroundColor = ConsoleColor.DarkGray;
			Console.WriteLine(new string('─', 74));
			Console.ResetColor();
		}

		private static void TableRow(params string[] cols)
		{
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.WriteLine(string.Join(" | ", cols));
			Console.ResetColor();
		}

		private static bool Confirm(string question)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.Write($"⚠️ {question} (y/n): ");
			Console.ResetColor();

			var s = (Console.ReadLine() ?? "").Trim();
			return s.Equals("y", StringComparison.OrdinalIgnoreCase) ||
				   s.Equals("yes", StringComparison.OrdinalIgnoreCase);
		}

		private static void ShowCategoriesMini()
		{
			var cats = categoryService.GetAll();
			if (cats.Count == 0)
			{
				Console.ForegroundColor = ConsoleColor.DarkYellow;
				Console.WriteLine("⚠️ No categories yet. Add a category first, then create a book.");
				Console.ResetColor();
				Console.WriteLine();
				return;
			}

			Console.ForegroundColor = ConsoleColor.DarkGray;
			Console.WriteLine("Available Categories:");
			Console.ResetColor();

			foreach (var c in cats)
				Console.WriteLine($"  🏷️ {c.Id} - {c.Name}");

			Console.WriteLine();
		}

		private static int ReadInt(string label)
		{
			Console.ForegroundColor = ConsoleColor.DarkYellow;
			Console.Write($"🔢 {label}: ");
			Console.ResetColor();

			if (!int.TryParse(Console.ReadLine(), out var v))
				throw new Exception("Invalid number.");
			return v;
		}

		private static string ReadStr(string label)
		{
			Console.ForegroundColor = ConsoleColor.DarkYellow;
			Console.Write($"✍️ {label}: ");
			Console.ResetColor();

			return Console.ReadLine() ?? "";
		}

		private static bool ReadBool(string label)
		{
			Console.ForegroundColor = ConsoleColor.DarkYellow;
			Console.Write($"🔘 {label}: ");
			Console.ResetColor();

			var s = (Console.ReadLine() ?? "").Trim();

			if (s == "1") return true;
			if (s == "0") return false;
			if (s.Equals("true", StringComparison.OrdinalIgnoreCase)) return true;
			if (s.Equals("false", StringComparison.OrdinalIgnoreCase)) return false;
			if (s.Equals("yes", StringComparison.OrdinalIgnoreCase)) return true;
			if (s.Equals("no", StringComparison.OrdinalIgnoreCase)) return false;

			throw new Exception("Invalid boolean. Use 1 or 0.");
		}

		private static void Pause()
		{
			Console.ForegroundColor = ConsoleColor.DarkGray;
			Console.WriteLine();
			Console.Write("Press ENTER...");
			Console.ResetColor();
			Console.ReadLine();
		}

		private static string PadRightSafe(string s, int width)
		{
			s ??= "";
			if (s.Length >= width) return s.Substring(0, width);
			return s.PadRight(width);
		}

		private static string Trunc(string s, int max)
		{
			s ??= "";
			if (s.Length <= max) return s;
			return s.Substring(0, Math.Max(0, max - 1)) + "…";
		}
	}
}
