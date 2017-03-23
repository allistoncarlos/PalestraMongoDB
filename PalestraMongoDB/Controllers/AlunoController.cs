using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Bson;
using Newtonsoft.Json;
using PalestraMongoDB.Domain;
using PalestraMongoDB.Model.Aluno;
using PalestraMongoDB.Model.Disciplina;
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

			var alunoRepository = new Repository<Aluno>(connectionStringConfiguration);
			var disciplinaRepository = new Repository<Disciplina>(connectionStringConfiguration);

			if (!string.IsNullOrEmpty(id))
				model = ToModel(await alunoRepository.Get(id));

			var disciplinas = await disciplinaRepository.Load();

			var selectList = new SelectList(disciplinas, "Id", "Nome");
			ViewBag.Disciplinas = selectList;

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

		public async Task<IActionResult> AddDisciplina(string id)
		{
			try
			{
				var repository = new Repository<Disciplina>(connectionStringConfiguration);

				var tempDisciplinas = new List<DisciplinaModel>();

				if (TempData.ContainsKey("Disciplinas"))
					tempDisciplinas = TempDeserialize<List<DisciplinaModel>>("Disciplinas");

				if (tempDisciplinas.SingleOrDefault(x => x.Id == id) == null)
				{
					var domain = await repository.Get(id);
					var model = new DisciplinaModel
					{
						Id = domain.Id.ToString(),
						Nome = domain.Nome,
						Duracao = domain.Duracao
					};

					tempDisciplinas.Add(model);
				}

				TempData.Remove("Disciplinas");
				TempData.Add("Disciplinas", TempSerialize<List<DisciplinaModel>>(tempDisciplinas));

				return PartialView("_DisciplinasTable", tempDisciplinas);
			}
			catch (Exception exc)
			{
				return PartialView("_DisciplinasTable");
			}
		}

		private AlunoModel ToModel(Aluno domain)
		{
			return new AlunoModel
			{
				Id = domain.Id.ToString(),
				Nome = domain.Nome,
				Email = domain.Email
			};
		}

		private Aluno ToDomain(AlunoModel model)
		{
			var aluno = new Aluno
			{
				Id = !string.IsNullOrEmpty(model.Id) ? ObjectId.Parse(model.Id) : ObjectId.GenerateNewId(DateTime.Now),
				Nome = model.Nome,
				Email = model.Email
			};

			if (TempData.ContainsKey("Disciplinas"))
			{
				var disciplinasModel = TempDeserialize<List<DisciplinaModel>>("Disciplinas");
				aluno.DisciplinasId = disciplinasModel.Select(disciplinaModel => disciplinaModel.Id).ToList();
			}

			return aluno;
		}

		private string TempSerialize<T>(T serializable)
		{
			return JsonConvert.SerializeObject(serializable);
		}

		private T TempDeserialize<T>(string tempDataKey)
		{
			var tempData = TempData[tempDataKey].ToString();
			return JsonConvert.DeserializeObject<T>(tempData);
		}
	}
}
