using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using PalestraMongoDB.Domain;
using PalestraMongoDB.Model.Aluno;
using PalestraMongoDB.Repository;

namespace PalestraMongoDB.Controllers
{
	public class AlunoController : Controller
	{
		private IConnectionStringConfiguration connectionStringConfiguration;

		public AlunoController(IConnectionStringConfiguration connectionStringConfiguration)
		{
			this.connectionStringConfiguration = connectionStringConfiguration;
		}

		public async Task<IActionResult> Index()
		{
			var repository = new Repository<Aluno>(connectionStringConfiguration);

			var domains = await repository.Load();
			var models = domains.Select(domain => ToModel(domain));

			return View(models.AsEnumerable());
		}

		public async Task<IActionResult> Edit(string id = null)
		{
			var model = new AlunoModel();

			var repository = new Repository<Aluno>(connectionStringConfiguration);

			if (!string.IsNullOrEmpty(id))
				model = ToModel(await repository.Get(id));

			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(AlunoModel model)
		{
			if (!ModelState.IsValid)
				return View(model);

			var repository = new Repository<Aluno>(connectionStringConfiguration);

			if (string.IsNullOrEmpty(model.Id))
				await repository.Save(ToDomain(model));
			else
				await repository.Update(ToDomain(model));

			return RedirectToAction("Index");
		}

		public async Task<IActionResult> Delete(string id)
		{
			var repository = new Repository<Aluno>(connectionStringConfiguration);

			await repository.Delete(id);

			return RedirectToAction("Index");
		}

		public AlunoModel ToModel(Aluno domain)
		{
			return new AlunoModel
			{
				Id = domain.Id.ToString(),
				Nome = domain.Nome,
				Email = domain.Email
			};
		}

		public Aluno ToDomain(AlunoModel model)
		{
			return new Aluno
			{
				Id = !string.IsNullOrEmpty(model.Id) ? ObjectId.Parse(model.Id) : ObjectId.GenerateNewId(DateTime.Now),
				Nome = model.Nome,
				Email = model.Email
			};
		}
	}
}
