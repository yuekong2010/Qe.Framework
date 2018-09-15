using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qe.Application.Base.Organization.User
{
    public class UserBLL : UserIBLL
    {
        #region 属性
        private UserService userService = new UserService();
        #endregion

        #region 缓存定义
        private ICache cache = CacheFactory.CaChe();
        private string cacheKeyPrivate = "elevator_user";//根据当前帐号和下级获得
                                                         //   private string cacheKey = "learun_adms_user_";       // +公司主键
        private string cacheKeyAccount = "learun_adms_user_account_";// +用户账号（账号不允许改动）
        private string cacheKeyId = "learun_adms_user_Id_";// +用户账号（账号不允许改动）
        #endregion

        #region 私有方法


        /// <summary>
        /// 递归查找，根据createuserid找到当前帐号的所有父帐号
        /// </summary>
        /// <param name="originUsers"></param>
        /// <param name="privateUsers"></param>
        /// <param name="createUserIds"></param>
        /// <returns></returns>
        private List<UserEntity> GetUserParent(List<UserEntity> originUsers, List<UserEntity> privateUsers, List<string> createUserIds)
        {
            List<string> parentUsers = new List<string>();
            foreach (var createUserId in createUserIds)
            {
                foreach (var originUser in originUsers)
                {
                    if (createUserId == originUser.F_UserId)
                    {
                        if (!privateUsers.Contains(originUser))
                        {
                            privateUsers.Add(originUser);
                            if (!string.IsNullOrEmpty(originUser.F_CreateUserId))
                                parentUsers.Add(originUser.F_CreateUserId);
                        }
                    }
                }
            }
            if (parentUsers.Count == 0)
            {
                return privateUsers;
            }
            else
            {
                return GetUserParent(originUsers, privateUsers, parentUsers);
            }
        }


        /// <summary>
        /// 递归查找，根据createuserid找到当前帐号的所有子帐号
        /// </summary>
        /// <param name="originUsers"></param>
        /// <param name="privateUsers"></param>
        /// <param name="createUserIds"></param>
        /// <returns></returns>
        private List<UserEntity> GetUserSon(List<UserEntity> originUsers, List<UserEntity> privateUsers, List<string> createUserIds)
        {
            List<string> sonUsers = new List<string>();
            foreach (var createUserId in createUserIds)
            {
                foreach (var originUser in originUsers)
                {
                    if (createUserId == originUser.F_CreateUserId)
                    {
                        if (!privateUsers.Contains(originUser))
                        {
                            privateUsers.Add(originUser);
                            sonUsers.Add(originUser.F_UserId);
                        }
                    }
                }
            }
            if (sonUsers.Count == 0)
            {
                return privateUsers;
            }
            else
            {
                return GetUserSon(originUsers, privateUsers, sonUsers);
            }
        }

        #endregion

        #region 获取数据
        /// <summary>
        /// 保存实体数据（新增、修改）
        /// <param name="keyValue">主键</param>
        /// <summary>
        /// <returns></returns>
        public string SaveEntityApp(string keyValue, UserEntity entity)
        {
            try
            {
                return userService.SaveEntityApp(keyValue, entity);
            }
            catch (Exception ex)
            {
                if (ex is ExceptionEx)
                {
                    throw;
                }
                else
                {
                    throw ExceptionEx.ThrowBusinessException(ex);
                }
            }
        }



        /// <summary>
        /// 获取APP技师页面显示列表数据
        /// <summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public IEnumerable<UserEntity> GetPageListAPP(Pagination pagination, string queryJson)
        {
            try
            {
                return userService.GetPageListAPP(pagination, queryJson);
            }
            catch (Exception ex)
            {
                if (ex is ExceptionEx)
                {
                    throw;
                }
                else
                {
                    throw ExceptionEx.ThrowBusinessException(ex);
                }
            }
        }

        /// <summary>
        /// 获取Valve_Technician表实体数据
        /// <param name="keyValue">主键</param>
        /// <summary>
        /// <returns></returns>
        public UserEntity GetValve_TechnicianEntity(string keyValue)
        {
            try
            {
                return userService.GetValve_TechnicianEntity(keyValue);
            }
            catch (Exception ex)
            {
                if (ex is ExceptionEx)
                {
                    throw;
                }
                else
                {
                    throw ExceptionEx.ThrowBusinessException(ex);
                }
            }
        }




        /*****************************Web本身********************************/
        /// <summary>
        /// 用户列表,获取当前账号的所有父账号们
        /// </summary>
        /// <returns></returns>
        public List<UserEntity> GetParentList()
        {
            try
            {
                List<UserEntity> list = cache.Read<List<UserEntity>>(cacheKeyPrivate, CacheId.user);
                if (list == null)
                {
                    list = (List<UserEntity>)userService.GetList();
                    cache.Write<List<UserEntity>>(cacheKeyPrivate, list, CacheId.user);
                }
                List<UserEntity> privateUsers = new List<UserEntity>();
                UserInfo userInfo = LoginUserInfo.Get();
                List<string> createUsers = list.Where(t => t.F_UserId == userInfo.userId).Select(t => t.F_CreateUserId).ToList();
                var result = GetUserParent(list, privateUsers, createUsers);
                return result;
            }
            catch (Exception ex)
            {
                if (ex is ExceptionEx)
                {
                    throw;
                }
                else
                {
                    throw ExceptionEx.ThrowBusinessException(ex);
                }
            }
        }


        /// <summary>
        /// 用户列表,获取当前账号的所有子账号们
        /// </summary>
        /// <returns></returns>
        public List<UserEntity> GetList(string keyword)
        {
            try
            {
                List<UserEntity> list = cache.Read<List<UserEntity>>(cacheKeyPrivate, CacheId.user);
                if (list == null)
                {
                    list = (List<UserEntity>)userService.GetList();
                    cache.Write<List<UserEntity>>(cacheKeyPrivate, list, CacheId.user);
                }
                List<UserEntity> privateUsers = new List<UserEntity>();
                UserInfo userInfo = LoginUserInfo.Get();
                var result = GetUserSon(list, privateUsers, new List<string>() { userInfo.userId });
                if (!string.IsNullOrEmpty(keyword))
                {
                    result = result.Where(t => t.F_Account.Contains(keyword) || t.F_RealName.Contains(keyword)).ToList();
                }
                return result;
            }
            catch (Exception ex)
            {
                if (ex is ExceptionEx)
                {
                    throw;
                }
                else
                {
                    throw ExceptionEx.ThrowBusinessException(ex);
                }
            }
        }


        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="keyword">查询关键词</param>
        /// <returns></returns>
        public List<UserEntity> GetPageList(Pagination pagination, string keyword)
        {
            try
            {
                List<UserEntity> list = GetList("");
                if (!string.IsNullOrEmpty(keyword))
                {
                    list = list.FindAll(t => t.F_RealName.ContainsEx(keyword) || t.F_Account.ContainsEx(keyword) || t.F_Mobile.ContainsEx(keyword));
                }
                list = list.FindPage<UserEntity>(pagination);
                return list;
            }
            catch (Exception ex)
            {
                if (ex is ExceptionEx)
                {
                    throw;
                }
                else
                {
                    throw ExceptionEx.ThrowBusinessException(ex);
                }
            }
        }

        /// <summary>
        /// 用户列表（导出Excel）
        /// </summary>
        /// <returns></returns>
        public void GetExportList()
        {
            try
            {
                //取出数据源
                DataTable exportTable = userService.GetExportList();
                //设置导出格式
                ExcelConfig excelconfig = new ExcelConfig();
                excelconfig.Title = "测试用户导出";
                excelconfig.TitleFont = "微软雅黑";
                excelconfig.TitlePoint = 25;
                excelconfig.FileName = "用户导出.xls";
                excelconfig.IsAllSizeColumn = true;
                //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
                excelconfig.ColumnEntity = new List<ColumnModel>();
                excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "f_account", ExcelColumn = "账户" });
                excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "f_realname", ExcelColumn = "姓名" });
                excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "f_gender", ExcelColumn = "性别" });
                excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "f_birthday", ExcelColumn = "生日" });
                excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "f_mobile", ExcelColumn = "手机", Background = Color.Red });
                excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "f_telephone", ExcelColumn = "电话", Background = Color.Red });
                excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "f_wechat", ExcelColumn = "微信" });
                excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "f_organize", ExcelColumn = "公司" });
                excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "f_department", ExcelColumn = "部门" });
                excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "f_description", ExcelColumn = "说明" });
                excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "f_createdate", ExcelColumn = "创建日期" });
                excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "f_createusername", ExcelColumn = "创建人" });
                //调用导出方法
                ExcelHelper.ExcelDownload(exportTable, excelconfig);
            }
            catch (Exception ex)
            {
                if (ex is ExceptionEx)
                {
                    throw;
                }
                else
                {
                    throw ExceptionEx.ThrowBusinessException(ex);
                }
            }
        }
        /// <summary>
        /// 获取实体,通过用户账号
        /// </summary>
        /// <param name="account">用户账号</param>
        /// <returns></returns>
        public UserEntity GetEntityByAccount(string account)
        {
            try
            {
                string userId = cache.Read<string>(cacheKeyAccount + account, CacheId.user);
                UserEntity userEntity;
                if (string.IsNullOrEmpty(userId))
                {
                    userEntity = userService.GetEntityByAccount(account);
                    if (userEntity != null)
                    {
                        cache.Write<string>(cacheKeyAccount + userEntity.F_Account, userEntity.F_UserId, CacheId.user);
                        cache.Write<UserEntity>(cacheKeyId + userEntity.F_UserId, userEntity, CacheId.user);
                    }
                }
                else
                {
                    userEntity = cache.Read<UserEntity>(cacheKeyId + userId, CacheId.user);
                    if (userEntity == null)
                    {
                        userEntity = userService.GetEntityByAccount(account);
                        if (userEntity != null)
                        {
                            cache.Write<UserEntity>(cacheKeyId + userEntity.F_UserId, userEntity, CacheId.user);
                        }
                    }
                }
                return userEntity;
            }
            catch (Exception ex)
            {
                if (ex is ExceptionEx)
                {
                    throw;
                }
                else
                {
                    throw ExceptionEx.ThrowBusinessException(ex);
                }
            }
        }
        /// <summary>
        /// 获取用户数据
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <returns></returns>
        public UserEntity GetEntityByUserId(string userId)
        {
            try
            {
                UserEntity userEntity = cache.Read<UserEntity>(cacheKeyId + userId, CacheId.user);
                if (userEntity == null)
                {
                    userEntity = userService.GetEntity(userId);
                    if (userEntity != null)
                    {
                        cache.Write<string>(cacheKeyAccount + userEntity.F_Account, userId, CacheId.user);
                        cache.Write<UserEntity>(cacheKeyId + userId, userEntity, CacheId.user);
                    }
                }
                return userEntity;
            }
            catch (Exception ex)
            {
                if (ex is ExceptionEx)
                {
                    throw;
                }
                else
                {
                    throw ExceptionEx.ThrowBusinessException(ex);
                }
            }
        }
        /// <summary>
        /// 获取用户列表数据
        /// </summary>
        /// <param name="userIds">用户主键串</param>
        /// <returns></returns>
        public List<UserEntity> GetListByUserIds(string userIds)
        {
            try
            {
                if (string.IsNullOrEmpty(userIds))
                {
                    return null;
                }
                List<UserEntity> list = new List<UserEntity>();
                string[] userList = userIds.Split(',');
                foreach (string userId in userList)
                {
                    UserEntity userEntity = GetEntityByUserId(userId);
                    list.Add(userEntity);
                }
                return list;
            }
            catch (Exception ex)
            {
                if (ex is ExceptionEx)
                {
                    throw;
                }
                else
                {
                    throw ExceptionEx.ThrowBusinessException(ex);
                }
            }
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 虚拟删除
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void VirtualDelete(string keyValue)
        {
            try
            {
                UserEntity userEntity = GetEntityByUserId(keyValue);
                //     cache.Remove(cacheKey + userEntity.F_CompanyId, CacheId.user);
                cache.Remove(cacheKeyId + keyValue, CacheId.user);
                cache.Remove(cacheKeyPrivate, CacheId.user);
                userService.VirtualDelete(keyValue);
            }
            catch (Exception ex)
            {
                if (ex is ExceptionEx)
                {
                    throw;
                }
                else
                {
                    throw ExceptionEx.ThrowBusinessException(ex);
                }
            }
        }
        /// <summary>
        /// 保存用户表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="userEntity">用户实体</param>
        /// <returns></returns>
        public void SaveEntity(string keyValue, UserEntity userEntity)
        {
            try
            {
                //   cache.Remove(cacheKey + userEntity.F_CompanyId, CacheId.user);
                cache.Remove(cacheKeyId + keyValue, CacheId.user);
                cache.Remove(cacheKeyPrivate, CacheId.user);
                if (!string.IsNullOrEmpty(keyValue))
                {
                    userEntity.F_Account = null;// 账号不允许改动
                }
                userService.SaveEntity(keyValue, userEntity);
            }
            catch (Exception ex)
            {
                if (ex is ExceptionEx)
                {
                    throw;
                }
                else
                {
                    throw ExceptionEx.ThrowBusinessException(ex);
                }
            }
        }
        /// <summary>
        /// 修改用户登录密码
        /// </summary>
        /// <param name="newPassword">新密码（MD5 小写）</param>
        /// <param name="oldPassword">旧密码（MD5 小写）</param>
        public bool RevisePassword(string newPassword, string oldPassword)
        {
            try
            {
                UserInfo userInfo = LoginUserInfo.Get();
                cache.Remove(cacheKeyId + userInfo.userId, CacheId.user);
                string oldPasswordByEncrypt = Md5Helper.Encrypt(DESEncrypt.Encrypt(oldPassword, userInfo.secretkey).ToLower(), 32).ToLower();
                if (oldPasswordByEncrypt == userInfo.password)
                {
                    userService.RevisePassword(userInfo.userId, newPassword);
                }
                else
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                if (ex is ExceptionEx)
                {
                    throw;
                }
                else
                {
                    throw ExceptionEx.ThrowBusinessException(ex);
                }
            }
        }
        /// <summary>
        /// 重置密码(000000)
        /// </summary>
        /// <param name="keyValue">账号主键</param>
        public void ResetPassword(string keyValue)
        {
            try
            {
                cache.Remove(cacheKeyId + keyValue, CacheId.user);
                string password = Md5Helper.Encrypt("000000", 32).ToLower();
                userService.RevisePassword(keyValue, password);
            }
            catch (Exception ex)
            {
                if (ex is ExceptionEx)
                {
                    throw;
                }
                else
                {
                    throw ExceptionEx.ThrowBusinessException(ex);
                }
            }
        }
        /// <summary>
        /// 修改用户状态
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="state">状态：1-启动；0-禁用</param>
        public void UpdateState(string keyValue, int state)
        {
            try
            {
                UserEntity userEntity = GetEntityByUserId(keyValue);
                //   cache.Remove(cacheKey + userEntity.F_CompanyId, CacheId.user);
                cache.Remove(cacheKeyId + keyValue, CacheId.user);
                cache.Remove(cacheKeyPrivate, CacheId.user);
                userService.UpdateState(keyValue, state);
            }
            catch (Exception ex)
            {
                if (ex is ExceptionEx)
                {
                    throw;
                }
                else
                {
                    throw ExceptionEx.ThrowBusinessException(ex);
                }
            }
        }
        #endregion

        #region 验证数据
        /// <summary>
        /// 账户不能重复
        /// </summary>
        /// <param name="account">账户值</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public bool ExistAccount(string account, string keyValue)
        {
            try
            {
                return userService.ExistAccount(account, keyValue);
            }
            catch (Exception ex)
            {
                if (ex is ExceptionEx)
                {
                    throw;
                }
                else
                {
                    throw ExceptionEx.ThrowBusinessException(ex);
                }
            }
        }
        #endregion

        #region 扩展方法
        /// <summary>
        /// 验证登录
        /// </summary>
        /// <param name="account">账号</param>
        /// <param name="password">密码 MD5 32位 小写</param>
        /// <returns></returns>
        public UserEntity CheckLogin(string account, string password, string userType = "Web")
        {
            try
            {
                UserEntity userEntity = GetEntityByAccount(account);
                if (userEntity == null)
                {
                    userEntity = new UserEntity()
                    {
                        LoginMsg = "账户不存在,错误代码01!",
                        LoginOk = false
                    };
                    return userEntity;
                }
                if (userEntity.F_UserType != userType)
                {
                    userEntity = new UserEntity()
                    {
                        LoginMsg = "账户不存在，错误代码02!",
                        LoginOk = false
                    };
                    return userEntity;
                }
                userEntity.LoginOk = false;
                if (userEntity.F_EnabledMark == 1)
                {
                    string dbPassword = Md5Helper.Encrypt(DESEncrypt.Encrypt(password.ToLower(), userEntity.F_Secretkey).ToLower(), 32).ToLower();
                    if (dbPassword == userEntity.F_Password)
                    {
                        userEntity.LoginOk = true;
                    }
                    else
                    {
                        userEntity.LoginMsg = "密码和账户名不匹配!";
                    }
                }
                else
                {
                    userEntity.LoginMsg = "账户被系统锁定,请联系管理员!";
                }
                return userEntity;
            }
            catch (Exception ex)
            {
                if (ex is ExceptionEx)
                {
                    throw;
                }
                else
                {
                    throw ExceptionEx.ThrowBusinessException(ex);
                }
            }
        }
        #endregion
    }
}
