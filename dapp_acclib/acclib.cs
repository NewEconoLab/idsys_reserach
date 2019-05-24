using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework.Services.System;
using Helper = Neo.SmartContract.Framework.Helper;
using System;
using System.ComponentModel;
using System.Numerics;

namespace dapp_nnc
{
    public class acclib : SmartContract
    {
        /*存储结构有     
         * map(rootid,pubkeyid)   账号=>pubkey
        */
        public static object Main(string method, object[] args)
        {
            var magicstr = "acclib ver 0.04_20190516";
            //必须在入口函数取得callscript，调用脚本的函数，也会导致执行栈变化，再取callscript就晚了
            var callscript = ExecutionEngine.CallingScriptHash;

            //this is in nep5
            if (method == "name")
            {
                return magicstr;
            }
            if (method == "create")
            {
                if (args.Length != 2) return false;
                byte[] id = (byte[])args[0];
                byte[] pubkey = (byte[])args[1];//a pubkey

                return create(id, pubkey);
            }
            if (method == "getkey")//得到key，用于进一步验证，比如转账验证
            {
                if (args.Length != 1) return false;
                byte[] id = (byte[])args[0];
                return getkey(id);
            }
            if (method == "getkey_scripthash")//得到key对应的scripthash
            {
                if (args.Length != 1) return false;
                byte[] id = (byte[])args[0];
                return getkey_scripthash(id);
            }
            if (method == "updatekey")//改密码
            {
                if (args.Length != 2) return false;
                byte[] id = (byte[])args[0];
                byte[] pubkey = (byte[])args[1];
                return updatekey(id, pubkey);
            }
            return false;
        }

        static byte[] getkey(byte[] id)
        {
            return _get(id);
        }
        static byte[] getkey_scripthash(byte[] id)
        {
            var pubkey = getkey(id);
            //用pubkey 计算出scripthash，因为地址是scripthash处理了一下
            //scripthash 和用户地址可以互相转换
            byte[] head = { 33 };
            byte[] end = { 0xac };
            byte[] script = Helper.Concat(head, pubkey);
            script = Helper.Concat(script, end);
            byte[] scripthash = SmartContract.Hash160(script);
            return scripthash;
        }
        static bool create(byte[] id, byte[] pubkey)
        {
            if (id.Length == 0)
                return false;
            //neo pubkey = 33
            if (pubkey.Length != 33)
                return false;
            //已存在不能创建两次
            var expubkey = getkey(id);
            if (expubkey.Length > 0)
                return false;

            _store(id, pubkey);
            return true;
        }
        static bool updatekey(byte[] id, byte[] newpubkey)
        {
            if (id.Length == 0)
                return false;
            //neo pubkey = 33
            if (newpubkey.Length != 33)
                return false;
            //验证旧的密码
            var scripthash = getkey_scripthash(id);
            if (!Runtime.CheckWitness(scripthash))
                return false;

            _store(id, newpubkey);
            return true;
        }
        static byte[] _get(byte[] id)
        {
            var key = id;
            return Storage.Get(Storage.CurrentContext, key);
        }
        static void _store(byte[] id, byte[] pubkey)
        {
            var key = id;
            Storage.Put(Storage.CurrentContext, key, pubkey);
        }

    }

}
