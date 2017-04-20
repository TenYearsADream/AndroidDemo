﻿using BJWater.Contract.Model;
using bjwaterAPI.Areas.EduInfo.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebInterface.Areas.monitor.Controllers
{
    public class CtController : BaseController
    {
        public HttpResponseMessage Get()
        {
            var paikelists = this.IHSService.SelectAllControler();
            return Request.CreateResponse(HttpStatusCode.OK, paikelists);

        }
        public HttpResponseMessage Get(int currentPage, int itemsPerPage)                     //分页
        {
            var model = this.IHSService.SelectAllControler();
            int totalCount = model.Count;
            if (model != null)
            {
                model = model.OrderByDescending(t => t.ID).Skip((currentPage - 1) * itemsPerPage).Take(itemsPerPage).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, new { model, totalCount });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
        }
        public HttpResponseMessage Post([FromBody]Controler model)
        {
            if ((string.IsNullOrEmpty(model.HttpMethod)) || (model.HttpMethod.ToUpper() == "POST"))
            {
                try
                {
                    this.IHSService.InsertControler(model);
                }
                catch
                {
                    return Request.CreateResponse(HttpStatusCode.NotModified);
                }
            }
            else if (model.HttpMethod.ToUpper() == "PUT")
            {
                return PUT(model);
            }
            else if (model.HttpMethod.ToUpper() == "DELETE")
            {
                return DELETE(model);
            }
            return Request.CreateResponse(HttpStatusCode.MethodNotAllowed);
        }
        public HttpResponseMessage DELETE(Controler model)
        {
            if (model != null && model.Remark != null)
            {
                try
                {
                    List<int> ids = Array.ConvertAll<string, int>(model.Remark.Split(new char[] { ',' }), s => int.Parse(s)).ToList();
                    this.IHSService.DeleteControler(ids);
                }
                catch
                {
                    return Request.CreateResponse(HttpStatusCode.NotModified);
                }
                return Request.CreateResponse(HttpStatusCode.OK, "删除成功！");
            }
            else
                return Request.CreateResponse(HttpStatusCode.NotImplemented, "请确认输入正确！");
        }
        public HttpResponseMessage PUT(Controler model)
        {
            if (model != null && model.ID != 0)
            {
                var paike = this.IHSService.SelectMonitor(model.ID);
                if (paike != null)
                {
                    try
                    {
                        this.IHSService.UpdateControler(model);
                    }
                    catch
                    {
                        return Request.CreateResponse(HttpStatusCode.NotModified);
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, "更新成功！");
                }
                else
                    return Request.CreateResponse(HttpStatusCode.NotFound, "未能找到该信息片！");
            }
            else
                return Request.CreateResponse(HttpStatusCode.NotImplemented, "操作错误！");
        }
    }
}