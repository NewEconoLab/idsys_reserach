using System;

namespace eventserver
{
    class Program
    {
        //test info
        // wif L2CmHCqgeNHL1i9XFhTLzUXsdr5LGjag4d56YY98FqEi4j5d83Mv
        // pubkey 02bf055764de0320c8221920d856d3d9b93dfc1dcbc759a560fd42553aa025ba5c
        // testid 0011
        //create ["(str)create",["(bytes)0011","(bytes)02bf055764de0320c8221920d856d3d9b93dfc1dcbc759a560fd42553aa025ba5c"]]
        //0xe4ff9ee2c639389a04c31fbaa9b87f8906b981c7cc3f7b8233bcb4dec037b470 succ
        //0xee503208c6c79afb7483d70601dbbdd52ef133ae2868a8bda325f0186c53e3e8 fail haveid

        //["(str)getkey",["(bytes)0011"]]
        //["(str)getkey_scripthash",["(bytes)0011"]]//f25d29b0059a3feecd3862fea44fdb4351a67755
        //0.03测试通过

        //checkkey

        //updatekey 需要构造特别交易，无法用neoray测试了

        //accmgr ver 0x89d71f0d96ae9538d56090d1660835d8ea5457d1
        //0.02 0xa7782e8d19d302fc32c366e31e0ca1fe67a9907a
        //0.03 0xd05b171be5c485888378bcf5c362df84995c5e92
        //0.04b 0xd29efab1d0d686588f5ac312348b5e9c65e3c829

        //id server 是一個server，我可以在這上面申請一個ID
        //這個ID 有一個RootKey，可以被改變
        //這個ID 有一個RootID，不會改變，ID確定，就確定

        //id<-->rootid 一對一

        //根據 RootKey/RootPubkey
        //產生多個不同鏈的key/pubkey
        //key_neo/pubkey_neo
        //key_eth/pubkey_eth
        //根據RootID
        //產生多個不同區塊鏈上的地址
        //address_neo
        //address_eth
        //每個鏈上存儲一份map《rooid,pubkey_thischain>
        //login 
        //去指定練上執行合約，login rootid，會產生一條login信息需要簽名
        //用pubkey_thischain 對應的簽名就可以，簽名可以切換成其他方式
        //轉賬
        //從這個地址中轉帳，需要key_neo的簽名
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}
