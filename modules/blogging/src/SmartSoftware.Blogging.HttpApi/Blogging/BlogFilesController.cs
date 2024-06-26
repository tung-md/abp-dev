﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SmartSoftware;
using SmartSoftware.AspNetCore.Mvc;
using SmartSoftware.Content;
using SmartSoftware.Blogging.Files;

namespace SmartSoftware.Blogging
{
    [RemoteService(Name = BloggingRemoteServiceConsts.RemoteServiceName)]
    [Area(BloggingRemoteServiceConsts.ModuleName)]
    [Route("api/blogging/files")]
    public class BlogFilesController : SmartSoftwareControllerBase, IFileAppService
    {
        private readonly IFileAppService _fileAppService;

        public BlogFilesController(IFileAppService fileAppService)
        {
            _fileAppService = fileAppService;
        }

        [HttpGet]
        [Route("{name}")]
        public Task<RawFileDto> GetAsync(string name) //TODO: output cache would be good
        {
            return _fileAppService.GetAsync(name);
        }

        [HttpGet]
        [Route("www/{name}")]
        public virtual async Task<IRemoteStreamContent> GetFileAsync(string name)
        {
            return await _fileAppService.GetFileAsync(name);
        }

        [HttpPost]
        [Route("images/upload")]
        public Task<FileUploadOutputDto> CreateAsync(FileUploadInputDto input)
        {
            return _fileAppService.CreateAsync(input);
        }
    }
}
