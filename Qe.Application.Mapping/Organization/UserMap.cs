using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qe.Application.Mapping
{
    /// <summary>
    /// 版 本 
    /// Copyright (c) 
    /// 创建人：
    /// 日 期：
    /// 描 述：用户数据库实体映射
    /// </summary>
    public class UserMap : EntityTypeConfiguration<UserEntity>
    {
        /// <summary>
        /// 用户数据库实体映射
        /// </summary>
        public UserMap()
        {
            #region 表、主键
            //表
            this.ToTable("Qe_BASE_USER");
            //主键
            this.HasKey(t => t.F_UserId);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
