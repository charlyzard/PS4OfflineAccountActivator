using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS4OfflineAccountActivator
{
    public class Registry
    {

        public const int SIZE_account_id = 8;
        public const int SIZE_NP_env = 17;
        public const int SIZE_login_flag = 4;


        public int Get_Entity_Number(int a, int b, int c, int d, int e)
        {
            if (a < 1 || a > b)
            {
                return e;
            }
            return (a - 1) * c + d;
        }


        public int KEY_account_id(int a)
        {
            return Get_Entity_Number(a, 16, 65536, 125830400, 125829120);
        }

        public int KEY_NP_env(int a)
        {
            return Get_Entity_Number(a, 16, 65536, 125874183, 125874176);
        }

        public int KEY_login_flag(int a)
        {
            return Get_Entity_Number(a, 16, 65536, 125831168, 125829120);
        }



    }

}
