﻿// Copyright (c) Polyrific, Inc 2018. All rights reserved.

using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Polyrific.Catapult.Cli.Extensions;
using Polyrific.Catapult.Shared.Service;

namespace Polyrific.Catapult.Cli.Commands.Model
{
    [Command("get", Description = "Get a single project data model record")]
    public class GetCommand : BaseCommand
    {
        private readonly IProjectService _projectService;
        private readonly IProjectDataModelService _projectDataModelService;

        public GetCommand(IConsole console, ILogger<GetCommand> logger,
            IProjectService projectService, IProjectDataModelService projectDataModelService) : base(console, logger)
        {
            _projectService = projectService;
            _projectDataModelService = projectDataModelService;
        }

        [Required]
        [Option("-p|--project <PROJECT>", "Name of the project", CommandOptionType.SingleValue)]
        public string Project { get; set; }

        [Required]
        [Option("-n|--name <NAME>", "Name of the data model", CommandOptionType.SingleValue)]
        public string Name { get; set; }

        public override string Execute()
        {
            Console.WriteLine($"Trying to get data model {Name} in project {Project}...");

            string message;

            var project = _projectService.GetProjectByName(Project).Result;
            if (project != null)
            {
                var model = _projectDataModelService.GetProjectDataModelByName(project.Id, Name).Result;

                if (model != null)
                {
                    message = model.ToCliString($"Data model {Name}", excludedFields: new string[]
                        {
                            "ProjectId",
                            "ProjectDataModelId",
                            "RelatedProjectDataModelId"
                        });
                    return message;
                }                
            }

            message = $"Failed to get model {Name}. Make sure the project and model names are correct.";

            return message;
        }
    }
}
