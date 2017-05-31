/* ***********************************************
 * author :  韩奎奎
 * function: 
 * history:  created by 韩奎奎 2017/4/24 9:38:10
 * ***********************************************/

namespace Han.Fm.Service.Sys
{
    using Dal.Sys;
    using Model.BaseDto;
    using Model.Dto.Sys;
    using Model.Table;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class UserService
    {
        private readonly UserDao userDao = new UserDao();

        public Response<List<UserResult>> GetUsers()
        {
            var result = new Response<List<UserResult>>();

            result.Result = userDao.GetUsers();

            return result;
        }

        public Response<bool> RemoveUser(string userId)
        {
            var result = new Response<bool>();
            List<User> users = new List<User>();
            users.Add(new User { Id = userId});

            userDao.BatchDelete(users, "id=?0", u=>u.Id);

            return result;

        }
    }
}
