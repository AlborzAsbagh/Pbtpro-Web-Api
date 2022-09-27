using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace WebApiNew.Models
{
    public class ResponseModel
    {
        [JsonProperty("status")]
        public bool Status { get; set; }
        [JsonProperty("error")]
        public bool Error { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("data")]
        public object Data { get; set; }
        [JsonProperty("count")]
        public int Count { get; set; }
        [JsonProperty("totalCount")]
        public int Total { get; set; }
        [JsonProperty("page")]
        public int Page { get; set; }
        [JsonProperty("pageSize")]
        public int PageSize { get; set; }
        [JsonProperty("msgId")]
        public int MsgId { get; set; }
        [JsonProperty("hasExtra")]
        public bool HasExtra { get; set; }
    }
}