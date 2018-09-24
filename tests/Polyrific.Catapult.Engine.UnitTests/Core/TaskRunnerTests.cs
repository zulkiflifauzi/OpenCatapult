﻿// Copyright (c) Polyrific, Inc 2018. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using Moq;
using Polyrific.Catapult.Engine.Core;
using Polyrific.Catapult.Engine.Core.JobTasks;
using Polyrific.Catapult.Shared.Dto.Constants;
using Polyrific.Catapult.Shared.Dto.JobDefinition;
using Xunit;

namespace Polyrific.Catapult.Engine.UnitTests.Core
{
    public class TaskRunnerTests
    {
        private readonly Mock<ILogger<TaskRunner>> _logger;
        private readonly Mock<IBuildTask> _buildTask;
        private readonly Mock<ICloneTask> _cloneTask;
        private readonly Mock<IDeployTask> _deployTask;
        private readonly Mock<IDeployDbTask> _deployDbTask;
        private readonly Mock<IGenerateTask> _generateTask;
        private readonly Mock<IMergeTask> _mergeTask;
        private readonly Mock<IPublishArtifactTask> _publishArtifactTask;
        private readonly Mock<IPushTask> _pushTask;
        private readonly Mock<ITestTask> _testTask;
        private readonly JobTaskService _jobTaskService;
        private readonly List<JobTaskDefinitionDto> _data;

        public TaskRunnerTests()
        {
            _data = new List<JobTaskDefinitionDto>
            {
                new JobTaskDefinitionDto
                {
                    Id = 1, Name = "Generate web app", Type = JobTaskDefinitionType.Generate, JobDefinitionId = 1, Sequence = 1
                },
                new JobTaskDefinitionDto
                {
                    Id = 2, Name = "Push code to GitHub", Type = JobTaskDefinitionType.Push, JobDefinitionId = 1, Sequence = 2
                },
                new JobTaskDefinitionDto
                {
                    Id = 3, Name = "Build web app", Type = JobTaskDefinitionType.Build, JobDefinitionId = 1, Sequence = 3
                },
                new JobTaskDefinitionDto
                {
                    Id = 4, Name = "Deploy web app", Type = JobTaskDefinitionType.Deploy, JobDefinitionId = 1, Sequence = 4
                }
            };

            _logger = new Mock<ILogger<TaskRunner>>();

            _buildTask = new Mock<IBuildTask>();
            _buildTask.Setup(t => t.RunPreprocessingTask()).ReturnsAsync(new TaskRunnerResult());
            _buildTask.Setup(t => t.RunPostprocessingTask()).ReturnsAsync(new TaskRunnerResult());

            _cloneTask = new Mock<ICloneTask>();
            _cloneTask.Setup(t => t.RunPreprocessingTask()).ReturnsAsync(new TaskRunnerResult());
            _cloneTask.Setup(t => t.RunPostprocessingTask()).ReturnsAsync(new TaskRunnerResult());

            _deployTask = new Mock<IDeployTask>();
            _deployTask.Setup(t => t.RunPreprocessingTask()).ReturnsAsync(new TaskRunnerResult());
            _deployTask.Setup(t => t.RunPostprocessingTask()).ReturnsAsync(new TaskRunnerResult());

            _deployDbTask = new Mock<IDeployDbTask>();
            _deployDbTask.Setup(t => t.RunPreprocessingTask()).ReturnsAsync(new TaskRunnerResult());
            _deployDbTask.Setup(t => t.RunPostprocessingTask()).ReturnsAsync(new TaskRunnerResult());

            _generateTask = new Mock<IGenerateTask>();
            _generateTask.Setup(t => t.RunPreprocessingTask()).ReturnsAsync(new TaskRunnerResult());
            _generateTask.Setup(t => t.RunPostprocessingTask()).ReturnsAsync(new TaskRunnerResult());

            _mergeTask = new Mock<IMergeTask>();
            _mergeTask.Setup(t => t.RunPreprocessingTask()).ReturnsAsync(new TaskRunnerResult());
            _mergeTask.Setup(t => t.RunPostprocessingTask()).ReturnsAsync(new TaskRunnerResult());

            _publishArtifactTask = new Mock<IPublishArtifactTask>();
            _publishArtifactTask.Setup(t => t.RunPreprocessingTask()).ReturnsAsync(new TaskRunnerResult());
            _publishArtifactTask.Setup(t => t.RunPostprocessingTask()).ReturnsAsync(new TaskRunnerResult());

            _pushTask = new Mock<IPushTask>();
            _pushTask.Setup(t => t.RunPreprocessingTask()).ReturnsAsync(new TaskRunnerResult());
            _pushTask.Setup(t => t.RunPostprocessingTask()).ReturnsAsync(new TaskRunnerResult());

            _testTask = new Mock<ITestTask>();
            _testTask.Setup(t => t.RunPreprocessingTask()).ReturnsAsync(new TaskRunnerResult());
            _testTask.Setup(t => t.RunPostprocessingTask()).ReturnsAsync(new TaskRunnerResult());

            _jobTaskService = new JobTaskService(_buildTask.Object, _cloneTask.Object, _deployTask.Object,
                _deployDbTask.Object, _generateTask.Object, _mergeTask.Object, _publishArtifactTask.Object,
                _pushTask.Object, _testTask.Object);
        }

        [Fact]
        public async void Run_SuccessAll()
        {
            _generateTask.Setup(t => t.RunMainTask(It.IsAny<Dictionary<string, string>>())).ReturnsAsync(new TaskRunnerResult(true, ""));
            _pushTask.Setup(t => t.RunMainTask(It.IsAny<Dictionary<string, string>>())).ReturnsAsync(new TaskRunnerResult(true, ""));
            _buildTask.Setup(t => t.RunMainTask(It.IsAny<Dictionary<string, string>>())).ReturnsAsync(new TaskRunnerResult(true, ""));
            _deployTask.Setup(t => t.RunMainTask(It.IsAny<Dictionary<string, string>>())).ReturnsAsync(new TaskRunnerResult(true, ""));
            
            var runner = new TaskRunner(_jobTaskService, _logger.Object);
            var results = await runner.Run(1, "20180817.1", _data, Path.Combine(AppContext.BaseDirectory, "plugins"));

            Assert.Equal(_data.Count, results.Count);
            Assert.True(results[1].IsSuccess);
            Assert.True(results[2].IsSuccess);
            Assert.True(results[3].IsSuccess);
            Assert.True(results[4].IsSuccess);
        }

        [Fact]
        public async void Run_FailedOne_SkipTheRests()
        {
            _generateTask.Setup(t => t.RunMainTask(It.IsAny<Dictionary<string, string>>())).ReturnsAsync(new TaskRunnerResult("Failed"));
            _pushTask.Setup(t => t.RunMainTask(It.IsAny<Dictionary<string, string>>())).ReturnsAsync(new TaskRunnerResult(true, ""));
            _buildTask.Setup(t => t.RunMainTask(It.IsAny<Dictionary<string, string>>())).ReturnsAsync(new TaskRunnerResult(true, ""));
            _deployTask.Setup(t => t.RunMainTask(It.IsAny<Dictionary<string, string>>())).ReturnsAsync(new TaskRunnerResult(true, ""));

            var runner = new TaskRunner(_jobTaskService, _logger.Object);
            var results = await runner.Run(1, "20180817.1", _data, Path.Combine(AppContext.BaseDirectory, "plugins"));

            Assert.Equal(_data.Count, results.Count);
            Assert.False(results[1].IsSuccess);
            Assert.False(results[2].IsProcessed);
            Assert.False(results[3].IsProcessed);
            Assert.False(results[4].IsProcessed);
        }

        [Fact]
        public async void Run__SuccessOne_FailedOne_SkipTheRests()
        {
            _generateTask.Setup(t => t.RunMainTask(It.IsAny<Dictionary<string, string>>())).ReturnsAsync(new TaskRunnerResult(true, ""));
            _pushTask.Setup(t => t.RunMainTask(It.IsAny<Dictionary<string, string>>())).ReturnsAsync(new TaskRunnerResult("Failed"));
            _buildTask.Setup(t => t.RunMainTask(It.IsAny<Dictionary<string, string>>())).ReturnsAsync(new TaskRunnerResult(true, ""));
            _deployTask.Setup(t => t.RunMainTask(It.IsAny<Dictionary<string, string>>())).ReturnsAsync(new TaskRunnerResult(true, ""));

            var runner = new TaskRunner(_jobTaskService, _logger.Object);
            var results = await runner.Run(1, "20180817.1", _data, Path.Combine(AppContext.BaseDirectory, "plugins"));

            Assert.Equal(_data.Count, results.Count);
            Assert.True(results[1].IsSuccess);
            Assert.False(results[2].IsSuccess);
            Assert.False(results[3].IsProcessed);
            Assert.False(results[4].IsProcessed);
        }

        [Fact]
        public async void Run__SuccessOne_FailedOneButContinue()
        {
            _generateTask.Setup(t => t.RunMainTask(It.IsAny<Dictionary<string, string>>())).ReturnsAsync(new TaskRunnerResult(true, ""));
            _pushTask.Setup(t => t.RunMainTask(It.IsAny<Dictionary<string, string>>())).ReturnsAsync(new TaskRunnerResult("Failed", false));
            _buildTask.Setup(t => t.RunMainTask(It.IsAny<Dictionary<string, string>>())).ReturnsAsync(new TaskRunnerResult(true, ""));
            _deployTask.Setup(t => t.RunMainTask(It.IsAny<Dictionary<string, string>>())).ReturnsAsync(new TaskRunnerResult(true, ""));

            var runner = new TaskRunner(_jobTaskService, _logger.Object);
            var results = await runner.Run(1, "20180817.1", _data, Path.Combine(AppContext.BaseDirectory, "plugins"));

            Assert.Equal(_data.Count, results.Count);
            Assert.True(results[1].IsSuccess);
            Assert.False(results[2].IsSuccess);
            Assert.True(results[3].IsProcessed);
            Assert.True(results[4].IsProcessed);
        }
    }
}
