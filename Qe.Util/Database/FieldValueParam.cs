using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qe.Util
{
    public class FieldValueParam
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 数据值
        /// </summary>
        public string value { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        public int type { get; set; }
    }
}
