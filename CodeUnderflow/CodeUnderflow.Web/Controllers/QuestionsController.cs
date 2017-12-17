﻿using CodeUnderflow.Common;
using CodeUnderflow.Common.Extensions;
using CodeUnderflow.Services.Contracts;
using CodeUnderflow.Services.Models.Questions;
using CodeUnderflow.Web.Models.QuestionsViewModels;
using Ganss.XSS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;

namespace CodeUnderflow.Web.Controllers
{
    public class QuestionsController : Controller
    {
        private IQuestionsService questionsService;
        private IUserService userService;
        private HtmlSanitizer htmlSanitizer;

        public QuestionsController(IQuestionsService questionsService, IUserService userService)
        {
            this.questionsService = questionsService;
            this.userService = userService;
            this.htmlSanitizer = new HtmlSanitizer();
            this.htmlSanitizer.AllowedAttributes.Add("class");
        }

        public IActionResult Index()
        {
            return Ok();
        }

        [HttpGet]
        [Authorize]
        public IActionResult New()
        {
            return View();
        }

        [HttpPost]
        public IActionResult New(NewQuestionModel newQuestionModel)
        {
            if (this.ModelState.IsValid)
            {
                int newQuestionId = this.questionsService.CreateNew(newQuestionModel.Title, newQuestionModel.Content, newQuestionModel.Tags, DateTime.UtcNow, this.User.GetUserId());

                return RedirectToAction(nameof(Details), new { id = newQuestionId });
            }

            return View(newQuestionModel);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Edit(int? id)
        {
            if (id != null
                && this.questionsService.Exists(id.Value)
                && (this.questionsService.IsAuthor(id.Value, this.User.GetUserId()) || this.User.IsInRole(GlobalConstants.AdminRoleName) || this.User.IsInRole(GlobalConstants.ModeratorRoleName)))
            {
                QuestionEditModel model = this.questionsService.GetEditInfo(id.Value);

                return View(model);
            }

            return Unauthorized();
        }

        [HttpPost]
        [Authorize]
        public IActionResult Edit(QuestionEditModel questionEditModel)
        {
            if (this.questionsService.Exists(questionEditModel.Id)
                && (this.questionsService.IsAuthor(questionEditModel.Id, this.User.GetUserId()) || this.User.IsInRole(GlobalConstants.AdminRoleName) || this.User.IsInRole(GlobalConstants.ModeratorRoleName)))
            {
                this.questionsService.Update(questionEditModel.Id, questionEditModel.Title, questionEditModel.Content, questionEditModel.Tags, DateTime.UtcNow);

                return RedirectToAction(nameof(Details), new { id = questionEditModel.Id });
            }

            return Unauthorized();
        }

        [HttpPost]
        [Authorize]
        public IActionResult Delete(int? id)
        {
            if (id != null && this.questionsService.Exists(id.Value)
                && (this.questionsService.IsAuthor(id.Value, this.User.GetUserId()) || this.User.IsInRole(GlobalConstants.AdminRoleName) || this.User.IsInRole(GlobalConstants.ModeratorRoleName)))
            {
                this.questionsService.Delete(id.Value);

                return RedirectToAction(nameof(Index));
            }

            return BadRequest();
        }

        [HttpGet]
        public IActionResult Details(int? id)
        {
            if (id != null && this.questionsService.Exists(id.Value))
            {
                QuestionDetailsModel model = this.questionsService.GetDetails(id.Value);

                model.Content = this.htmlSanitizer.Sanitize(model.Content);

                return View(model);
            }

            return BadRequest();
        }

        [HttpPost]
        [Authorize]
        public IActionResult Vote()
        {
            var headers = this.Request.Headers;

            if (headers.ContainsKey("QuestionId") && headers.ContainsKey("IsUpvote"))
            {
                if (int.TryParse(headers["QuestionId"], out int questionId) && bool.TryParse(headers["IsUpvote"], out bool isUpvote))
                {
                    if (this.questionsService.Exists(questionId))
                    {
                        
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }

            return BadRequest();
        }
    }
}