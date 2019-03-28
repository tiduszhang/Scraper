using MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Scraper.BAL
{
    /// <summary>
    /// 商品信息
    /// </summary>
    public class Commodity : NotifyBaseModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Commodity()
        {
            ID = Guid.NewGuid().ToString("N");
        }
        /// <summary>
        /// 
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 商品URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// html节点
        /// </summary>
        public HtmlElement HtmlElementItem { get; set; }

        /// <summary>
        /// 解析Item
        /// </summary>
        public void ParsingItem()
        {

        }
    }
}
