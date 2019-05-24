using Newtonsoft.Json.Linq;
using System.Numerics;
using ThinNeo;
using System.Linq;

namespace sdk
{
    public class Acc_ScriptPackage:ScriptPackage
    {
        public Acc_ScriptPackage(string _contractHash) : base(_contractHash)
        {

        }

        public byte[] GetScript_Name()
        {
            JArray inputJA = JArray.Parse(string.Format(@"
                    [
	                    '(str)name',
	                    [
	                    ]
                    ]"));
            return GetScript(contractHash, inputJA);
        }

        public byte[] GetScript_Create(string id,string pubKey)
        {
            JArray inputJA = JArray.Parse(string.Format(@"
                    [
	                    '(str)create',
	                    [
		                    '(str){0}',
                            '(bytes){1}'
	                    ]
                    ]",id,pubKey));
            return GetScript(contractHash, inputJA);
        }

        public byte[] GetScript_Getkey(string id)
        {
            JArray inputJA = JArray.Parse(string.Format(@"
                    [
	                    '(str)getkey',
	                    [
		                    '(str){0}'
	                    ]
                    ]", id));
            return GetScript(contractHash, inputJA);
        }

        public byte[] GetScript_Getkey_scripthash(string id)
        {
            JArray inputJA = JArray.Parse(string.Format(@"
                    [
	                    '(str)getkey_scripthash',
	                    [
		                    '(str){0}'
	                    ]
                    ]", id));
            return GetScript(contractHash, inputJA);
        }

        public byte[] GetScript_Updatekey(string id, string pubKey)
        {
            JArray inputJA = JArray.Parse(string.Format(@"
                    [
	                    '(str)updatekey',
	                    [
		                    '(str){0}',
                            '(bytes){1}'
	                    ]
                    ]", id, pubKey));
            return GetScript(contractHash, inputJA);
        }
    }
}
