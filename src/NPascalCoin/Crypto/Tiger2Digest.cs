using System;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Utilities;

namespace NPascalCoin.Crypto
{
    public class Tiger2Digest
        : IDigest, IMemoable
    {
        #region Consts

        private const ulong C1 = 0xA5A5A5A5A5A5A5A5;
        private const ulong C2 = 0x0123456789ABCDEF;

        private static readonly ulong[] St1 =
        {
            0x02AAB17CF7E90C5E, 0xAC424B03E243A8EC, 0x72CD5BE30DD5FCD3, 0x6D019B93F6F97F3A,
            0xCD9978FFD21F9193, 0x7573A1C9708029E2, 0xB164326B922A83C3, 0x46883EEE04915870,
            0xEAACE3057103ECE6, 0xC54169B808A3535C, 0x4CE754918DDEC47C, 0x0AA2F4DFDC0DF40C,
            0x10B76F18A74DBEFA, 0xC6CCB6235AD1AB6A, 0x13726121572FE2FF, 0x1A488C6F199D921E,
            0x4BC9F9F4DA0007CA, 0x26F5E6F6E85241C7, 0x859079DBEA5947B6, 0x4F1885C5C99E8C92,
            0xD78E761EA96F864B, 0x8E36428C52B5C17D, 0x69CF6827373063C1, 0xB607C93D9BB4C56E,
            0x7D820E760E76B5EA, 0x645C9CC6F07FDC42, 0xBF38A078243342E0, 0x5F6B343C9D2E7D04,
            0xF2C28AEB600B0EC6, 0x6C0ED85F7254BCAC, 0x71592281A4DB4FE5, 0x1967FA69CE0FED9F,
            0xFD5293F8B96545DB, 0xC879E9D7F2A7600B, 0x860248920193194E, 0xA4F9533B2D9CC0B3,
            0x9053836C15957613, 0xDB6DCF8AFC357BF1, 0x18BEEA7A7A370F57, 0x037117CA50B99066,
            0x6AB30A9774424A35, 0xF4E92F02E325249B, 0x7739DB07061CCAE1, 0xD8F3B49CECA42A05,
            0xBD56BE3F51382F73, 0x45FAED5843B0BB28, 0x1C813D5C11BF1F83, 0x8AF0E4B6D75FA169,
            0x33EE18A487AD9999, 0x3C26E8EAB1C94410, 0xB510102BC0A822F9, 0x141EEF310CE6123B,
            0xFC65B90059DDB154, 0xE0158640C5E0E607, 0x884E079826C3A3CF, 0x930D0D9523C535FD,
            0x35638D754E9A2B00, 0x4085FCCF40469DD5, 0xC4B17AD28BE23A4C, 0xCAB2F0FC6A3E6A2E,
            0x2860971A6B943FCD, 0x3DDE6EE212E30446, 0x6222F32AE01765AE, 0x5D550BB5478308FE,
            0xA9EFA98DA0EDA22A, 0xC351A71686C40DA7, 0x1105586D9C867C84, 0xDCFFEE85FDA22853,
            0xCCFBD0262C5EEF76, 0xBAF294CB8990D201, 0xE69464F52AFAD975, 0x94B013AFDF133E14,
            0x06A7D1A32823C958, 0x6F95FE5130F61119, 0xD92AB34E462C06C0, 0xED7BDE33887C71D2,
            0x79746D6E6518393E, 0x5BA419385D713329, 0x7C1BA6B948A97564, 0x31987C197BFDAC67,
            0xDE6C23C44B053D02, 0x581C49FED002D64D, 0xDD474D6338261571, 0xAA4546C3E473D062,
            0x928FCE349455F860, 0x48161BBACAAB94D9, 0x63912430770E6F68, 0x6EC8A5E602C6641C,
            0x87282515337DDD2B, 0x2CDA6B42034B701B, 0xB03D37C181CB096D, 0xE108438266C71C6F,
            0x2B3180C7EB51B255, 0xDF92B82F96C08BBC, 0x5C68C8C0A632F3BA, 0x5504CC861C3D0556,
            0xABBFA4E55FB26B8F, 0x41848B0AB3BACEB4, 0xB334A273AA445D32, 0xBCA696F0A85AD881,
            0x24F6EC65B528D56C, 0x0CE1512E90F4524A, 0x4E9DD79D5506D35A, 0x258905FAC6CE9779,
            0x2019295B3E109B33, 0xF8A9478B73A054CC, 0x2924F2F934417EB0, 0x3993357D536D1BC4,
            0x38A81AC21DB6FF8B, 0x47C4FBF17D6016BF, 0x1E0FAADD7667E3F5, 0x7ABCFF62938BEB96,
            0xA78DAD948FC179C9, 0x8F1F98B72911E50D, 0x61E48EAE27121A91, 0x4D62F7AD31859808,
            0xECEBA345EF5CEAEB, 0xF5CEB25EBC9684CE, 0xF633E20CB7F76221, 0xA32CDF06AB8293E4,
            0x985A202CA5EE2CA4, 0xCF0B8447CC8A8FB1, 0x9F765244979859A3, 0xA8D516B1A1240017,
            0x0BD7BA3EBB5DC726, 0xE54BCA55B86ADB39, 0x1D7A3AFD6C478063, 0x519EC608E7669EDD,
            0x0E5715A2D149AA23, 0x177D4571848FF194, 0xEEB55F3241014C22, 0x0F5E5CA13A6E2EC2,
            0x8029927B75F5C361, 0xAD139FABC3D6E436, 0x0D5DF1A94CCF402F, 0x3E8BD948BEA5DFC8,
            0xA5A0D357BD3FF77E, 0xA2D12E251F74F645, 0x66FD9E525E81A082, 0x2E0C90CE7F687A49,
            0xC2E8BCBEBA973BC5, 0x000001BCE509745F, 0x423777BBE6DAB3D6, 0xD1661C7EAEF06EB5,
            0xA1781F354DAACFD8, 0x2D11284A2B16AFFC, 0xF1FC4F67FA891D1F, 0x73ECC25DCB920ADA,
            0xAE610C22C2A12651, 0x96E0A810D356B78A, 0x5A9A381F2FE7870F, 0xD5AD62EDE94E5530,
            0xD225E5E8368D1427, 0x65977B70C7AF4631, 0x99F889B2DE39D74F, 0x233F30BF54E1D143,
            0x9A9675D3D9A63C97, 0x5470554FF334F9A8, 0x166ACB744A4F5688, 0x70C74CAAB2E4AEAD,
            0xF0D091646F294D12, 0x57B82A89684031D1, 0xEFD95A5A61BE0B6B, 0x2FBD12E969F2F29A,
            0x9BD37013FEFF9FE8, 0x3F9B0404D6085A06, 0x4940C1F3166CFE15, 0x09542C4DCDF3DEFB,
            0xB4C5218385CD5CE3, 0xC935B7DC4462A641, 0x3417F8A68ED3B63F, 0xB80959295B215B40,
            0xF99CDAEF3B8C8572, 0x018C0614F8FCB95D, 0x1B14ACCD1A3ACDF3, 0x84D471F200BB732D,
            0xC1A3110E95E8DA16, 0x430A7220BF1A82B8, 0xB77E090D39DF210E, 0x5EF4BD9F3CD05E9D,
            0x9D4FF6DA7E57A444, 0xDA1D60E183D4A5F8, 0xB287C38417998E47, 0xFE3EDC121BB31886,
            0xC7FE3CCC980CCBEF, 0xE46FB590189BFD03, 0x3732FD469A4C57DC, 0x7EF700A07CF1AD65,
            0x59C64468A31D8859, 0x762FB0B4D45B61F6, 0x155BAED099047718, 0x68755E4C3D50BAA6,
            0xE9214E7F22D8B4DF, 0x2ADDBF532EAC95F4, 0x32AE3909B4BD0109, 0x834DF537B08E3450,
            0xFA209DA84220728D, 0x9E691D9B9EFE23F7, 0x0446D288C4AE8D7F, 0x7B4CC524E169785B,
            0x21D87F0135CA1385, 0xCEBB400F137B8AA5, 0x272E2B66580796BE, 0x3612264125C2B0DE,
            0x057702BDAD1EFBB2, 0xD4BABB8EACF84BE9, 0x91583139641BC67B, 0x8BDC2DE08036E024,
            0x603C8156F49F68ED, 0xF7D236F7DBEF5111, 0x9727C4598AD21E80, 0xA08A0896670A5FD7,
            0xCB4A8F4309EBA9CB, 0x81AF564B0F7036A1, 0xC0B99AA778199ABD, 0x959F1EC83FC8E952,
            0x8C505077794A81B9, 0x3ACAAF8F056338F0, 0x07B43F50627A6778, 0x4A44AB49F5ECCC77,
            0x3BC3D6E4B679EE98, 0x9CC0D4D1CF14108C, 0x4406C00B206BC8A0, 0x82A18854C8D72D89,
            0x67E366B35C3C432C, 0xB923DD61102B37F2, 0x56AB2779D884271D, 0xBE83E1B0FF1525AF,
            0xFB7C65D4217E49A9, 0x6BDBE0E76D48E7D4, 0x08DF828745D9179E, 0x22EA6A9ADD53BD34,
            0xE36E141C5622200A, 0x7F805D1B8CB750EE, 0xAFE5C7A59F58E837, 0xE27F996A4FB1C23C,
            0xD3867DFB0775F0D0, 0xD0E673DE6E88891A, 0x123AEB9EAFB86C25, 0x30F1D5D5C145B895,
            0xBB434A2DEE7269E7, 0x78CB67ECF931FA38, 0xF33B0372323BBF9C, 0x52D66336FB279C74,
            0x505F33AC0AFB4EAA, 0xE8A5CD99A2CCE187, 0x534974801E2D30BB, 0x8D2D5711D5876D90,
            0x1F1A412891BC038E, 0xD6E2E71D82E56648, 0x74036C3A497732B7, 0x89B67ED96361F5AB,
            0xFFED95D8F1EA02A2, 0xE72B3BD61464D43D, 0xA6300F170BDC4820, 0xEBC18760ED78A77A
        };

        private static readonly ulong[] St2 =
        {
            0xE6A6BE5A05A12138, 0xB5A122A5B4F87C98, 0x563C6089140B6990, 0x4C46CB2E391F5DD5,
            0xD932ADDBC9B79434, 0x08EA70E42015AFF5, 0xD765A6673E478CF1, 0xC4FB757EAB278D99,
            0xDF11C6862D6E0692, 0xDDEB84F10D7F3B16, 0x6F2EF604A665EA04, 0x4A8E0F0FF0E0DFB3,
            0xA5EDEEF83DBCBA51, 0xFC4F0A2A0EA4371E, 0xE83E1DA85CB38429, 0xDC8FF882BA1B1CE2,
            0xCD45505E8353E80D, 0x18D19A00D4DB0717, 0x34A0CFEDA5F38101, 0x0BE77E518887CAF2,
            0x1E341438B3C45136, 0xE05797F49089CCF9, 0xFFD23F9DF2591D14, 0x543DDA228595C5CD,
            0x661F81FD99052A33, 0x8736E641DB0F7B76, 0x15227725418E5307, 0xE25F7F46162EB2FA,
            0x48A8B2126C13D9FE, 0xAFDC541792E76EEA, 0x03D912BFC6D1898F, 0x31B1AAFA1B83F51B,
            0xF1AC2796E42AB7D9, 0x40A3A7D7FCD2EBAC, 0x1056136D0AFBBCC5, 0x7889E1DD9A6D0C85,
            0xD33525782A7974AA, 0xA7E25D09078AC09B, 0xBD4138B3EAC6EDD0, 0x920ABFBE71EB9E70,
            0xA2A5D0F54FC2625C, 0xC054E36B0B1290A3, 0xF6DD59FF62FE932B, 0x3537354511A8AC7D,
            0xCA845E9172FADCD4, 0x84F82B60329D20DC, 0x79C62CE1CD672F18, 0x8B09A2ADD124642C,
            0xD0C1E96A19D9E726, 0x5A786A9B4BA9500C, 0x0E020336634C43F3, 0xC17B474AEB66D822,
            0x6A731AE3EC9BAAC2, 0x8226667AE0840258, 0x67D4567691CAECA5, 0x1D94155C4875ADB5,
            0x6D00FD985B813FDF, 0x51286EFCB774CD06, 0x5E8834471FA744AF, 0xF72CA0AEE761AE2E,
            0xBE40E4CDAEE8E09A, 0xE9970BBB5118F665, 0x726E4BEB33DF1964, 0x703B000729199762,
            0x4631D816F5EF30A7, 0xB880B5B51504A6BE, 0x641793C37ED84B6C, 0x7B21ED77F6E97D96,
            0x776306312EF96B73, 0xAE528948E86FF3F4, 0x53DBD7F286A3F8F8, 0x16CADCE74CFC1063,
            0x005C19BDFA52C6DD, 0x68868F5D64D46AD3, 0x3A9D512CCF1E186A, 0x367E62C2385660AE,
            0xE359E7EA77DCB1D7, 0x526C0773749ABE6E, 0x735AE5F9D09F734B, 0x493FC7CC8A558BA8,
            0xB0B9C1533041AB45, 0x321958BA470A59BD, 0x852DB00B5F46C393, 0x91209B2BD336B0E5,
            0x6E604F7D659EF19F, 0xB99A8AE2782CCB24, 0xCCF52AB6C814C4C7, 0x4727D9AFBE11727B,
            0x7E950D0C0121B34D, 0x756F435670AD471F, 0xF5ADD442615A6849, 0x4E87E09980B9957A,
            0x2ACFA1DF50AEE355, 0xD898263AFD2FD556, 0xC8F4924DD80C8FD6, 0xCF99CA3D754A173A,
            0xFE477BACAF91BF3C, 0xED5371F6D690C12D, 0x831A5C285E687094, 0xC5D3C90A3708A0A4,
            0x0F7F903717D06580, 0x19F9BB13B8FDF27F, 0xB1BD6F1B4D502843, 0x1C761BA38FFF4012,
            0x0D1530C4E2E21F3B, 0x8943CE69A7372C8A, 0xE5184E11FEB5CE66, 0x618BDB80BD736621,
            0x7D29BAD68B574D0B, 0x81BB613E25E6FE5B, 0x071C9C10BC07913F, 0xC7BEEB7909AC2D97,
            0xC3E58D353BC5D757, 0xEB017892F38F61E8, 0xD4EFFB9C9B1CC21A, 0x99727D26F494F7AB,
            0xA3E063A2956B3E03, 0x9D4A8B9A4AA09C30, 0x3F6AB7D500090FB4, 0x9CC0F2A057268AC0,
            0x3DEE9D2DEDBF42D1, 0x330F49C87960A972, 0xC6B2720287421B41, 0x0AC59EC07C00369C,
            0xEF4EAC49CB353425, 0xF450244EEF0129D8, 0x8ACC46E5CAF4DEB6, 0x2FFEAB63989263F7,
            0x8F7CB9FE5D7A4578, 0x5BD8F7644E634635, 0x427A7315BF2DC900, 0x17D0C4AA2125261C,
            0x3992486C93518E50, 0xB4CBFEE0A2D7D4C3, 0x7C75D6202C5DDD8D, 0xDBC295D8E35B6C61,
            0x60B369D302032B19, 0xCE42685FDCE44132, 0x06F3DDB9DDF65610, 0x8EA4D21DB5E148F0,
            0x20B0FCE62FCD496F, 0x2C1B912358B0EE31, 0xB28317B818F5A308, 0xA89C1E189CA6D2CF,
            0x0C6B18576AAADBC8, 0xB65DEAA91299FAE3, 0xFB2B794B7F1027E7, 0x04E4317F443B5BEB,
            0x4B852D325939D0A6, 0xD5AE6BEEFB207FFC, 0x309682B281C7D374, 0xBAE309A194C3B475,
            0x8CC3F97B13B49F05, 0x98A9422FF8293967, 0x244B16B01076FF7C, 0xF8BF571C663D67EE,
            0x1F0D6758EEE30DA1, 0xC9B611D97ADEB9B7, 0xB7AFD5887B6C57A2, 0x6290AE846B984FE1,
            0x94DF4CDEACC1A5FD, 0x058A5BD1C5483AFF, 0x63166CC142BA3C37, 0x8DB8526EB2F76F40,
            0xE10880036F0D6D4E, 0x9E0523C9971D311D, 0x45EC2824CC7CD691, 0x575B8359E62382C9,
            0xFA9E400DC4889995, 0xD1823ECB45721568, 0xDAFD983B8206082F, 0xAA7D29082386A8CB,
            0x269FCD4403B87588, 0x1B91F5F728BDD1E0, 0xE4669F39040201F6, 0x7A1D7C218CF04ADE,
            0x65623C29D79CE5CE, 0x2368449096C00BB1, 0xAB9BF1879DA503BA, 0xBC23ECB1A458058E,
            0x9A58DF01BB401ECC, 0xA070E868A85F143D, 0x4FF188307DF2239E, 0x14D565B41A641183,
            0xEE13337452701602, 0x950E3DCF3F285E09, 0x59930254B9C80953, 0x3BF299408930DA6D,
            0xA955943F53691387, 0xA15EDECAA9CB8784, 0x29142127352BE9A0, 0x76F0371FFF4E7AFB,
            0x0239F450274F2228, 0xBB073AF01D5E868B, 0xBFC80571C10E96C1, 0xD267088568222E23,
            0x9671A3D48E80B5B0, 0x55B5D38AE193BB81, 0x693AE2D0A18B04B8, 0x5C48B4ECADD5335F,
            0xFD743B194916A1CA, 0x2577018134BE98C4, 0xE77987E83C54A4AD, 0x28E11014DA33E1B9,
            0x270CC59E226AA213, 0x71495F756D1A5F60, 0x9BE853FB60AFEF77, 0xADC786A7F7443DBF,
            0x0904456173B29A82, 0x58BC7A66C232BD5E, 0xF306558C673AC8B2, 0x41F639C6B6C9772A,
            0x216DEFE99FDA35DA, 0x11640CC71C7BE615, 0x93C43694565C5527, 0xEA038E6246777839,
            0xF9ABF3CE5A3E2469, 0x741E768D0FD312D2, 0x0144B883CED652C6, 0xC20B5A5BA33F8552,
            0x1AE69633C3435A9D, 0x97A28CA4088CFDEC, 0x8824A43C1E96F420, 0x37612FA66EEEA746,
            0x6B4CB165F9CF0E5A, 0x43AA1C06A0ABFB4A, 0x7F4DC26FF162796B, 0x6CBACC8E54ED9B0F,
            0xA6B7FFEFD2BB253E, 0x2E25BC95B0A29D4F, 0x86D6A58BDEF1388C, 0xDED74AC576B6F054,
            0x8030BDBC2B45805D, 0x3C81AF70E94D9289, 0x3EFF6DDA9E3100DB, 0xB38DC39FDFCC8847,
            0x123885528D17B87E, 0xF2DA0ED240B1B642, 0x44CEFADCD54BF9A9, 0x1312200E433C7EE6,
            0x9FFCC84F3A78C748, 0xF0CD1F72248576BB, 0xEC6974053638CFE4, 0x2BA7B67C0CEC4E4C,
            0xAC2F4DF3E5CE32ED, 0xCB33D14326EA4C11, 0xA4E9044CC77E58BC, 0x5F513293D934FCEF,
            0x5DC9645506E55444, 0x50DE418F317DE40A, 0x388CB31A69DDE259, 0x2DB4A83455820A86,
            0x9010A91E84711AE9, 0x4DF7F0B7B1498371, 0xD62A2EABC0977179, 0x22FAC097AA8D5C0E
        };

        private static readonly ulong[] St3 =
        {
            0xF49FCC2FF1DAF39B, 0x487FD5C66FF29281, 0xE8A30667FCDCA83F, 0x2C9B4BE3D2FCCE63,
            0xDA3FF74B93FBBBC2, 0x2FA165D2FE70BA66, 0xA103E279970E93D4, 0xBECDEC77B0E45E71,
            0xCFB41E723985E497, 0xB70AAA025EF75017, 0xD42309F03840B8E0, 0x8EFC1AD035898579,
            0x96C6920BE2B2ABC5, 0x66AF4163375A9172, 0x2174ABDCCA7127FB, 0xB33CCEA64A72FF41,
            0xF04A4933083066A5, 0x8D970ACDD7289AF5, 0x8F96E8E031C8C25E, 0xF3FEC02276875D47,
            0xEC7BF310056190DD, 0xF5ADB0AEBB0F1491, 0x9B50F8850FD58892, 0x4975488358B74DE8,
            0xA3354FF691531C61, 0x0702BBE481D2C6EE, 0x89FB24057DEDED98, 0xAC3075138596E902,
            0x1D2D3580172772ED, 0xEB738FC28E6BC30D, 0x5854EF8F63044326, 0x9E5C52325ADD3BBE,
            0x90AA53CF325C4623, 0xC1D24D51349DD067, 0x2051CFEEA69EA624, 0x13220F0A862E7E4F,
            0xCE39399404E04864, 0xD9C42CA47086FCB7, 0x685AD2238A03E7CC, 0x066484B2AB2FF1DB,
            0xFE9D5D70EFBF79EC, 0x5B13B9DD9C481854, 0x15F0D475ED1509AD, 0x0BEBCD060EC79851,
            0xD58C6791183AB7F8, 0xD1187C5052F3EEE4, 0xC95D1192E54E82FF, 0x86EEA14CB9AC6CA2,
            0x3485BEB153677D5D, 0xDD191D781F8C492A, 0xF60866BAA784EBF9, 0x518F643BA2D08C74,
            0x8852E956E1087C22, 0xA768CB8DC410AE8D, 0x38047726BFEC8E1A, 0xA67738B4CD3B45AA,
            0xAD16691CEC0DDE19, 0xC6D4319380462E07, 0xC5A5876D0BA61938, 0x16B9FA1FA58FD840,
            0x188AB1173CA74F18, 0xABDA2F98C99C021F, 0x3E0580AB134AE816, 0x5F3B05B773645ABB,
            0x2501A2BE5575F2F6, 0x1B2F74004E7E8BA9, 0x1CD7580371E8D953, 0x7F6ED89562764E30,
            0xB15926FF596F003D, 0x9F65293DA8C5D6B9, 0x6ECEF04DD690F84C, 0x4782275FFF33AF88,
            0xE41433083F820801, 0xFD0DFE409A1AF9B5, 0x4325A3342CDB396B, 0x8AE77E62B301B252,
            0xC36F9E9F6655615A, 0x85455A2D92D32C09, 0xF2C7DEA949477485, 0x63CFB4C133A39EBA,
            0x83B040CC6EBC5462, 0x3B9454C8FDB326B0, 0x56F56A9E87FFD78C, 0x2DC2940D99F42BC6,
            0x98F7DF096B096E2D, 0x19A6E01E3AD852BF, 0x42A99CCBDBD4B40B, 0xA59998AF45E9C559,
            0x366295E807D93186, 0x6B48181BFAA1F773, 0x1FEC57E2157A0A1D, 0x4667446AF6201AD5,
            0xE615EBCACFB0F075, 0xB8F31F4F68290778, 0x22713ED6CE22D11E, 0x3057C1A72EC3C93B,
            0xCB46ACC37C3F1F2F, 0xDBB893FD02AAF50E, 0x331FD92E600B9FCF, 0xA498F96148EA3AD6,
            0xA8D8426E8B6A83EA, 0xA089B274B7735CDC, 0x87F6B3731E524A11, 0x118808E5CBC96749,
            0x9906E4C7B19BD394, 0xAFED7F7E9B24A20C, 0x6509EADEEB3644A7, 0x6C1EF1D3E8EF0EDE,
            0xB9C97D43E9798FB4, 0xA2F2D784740C28A3, 0x7B8496476197566F, 0x7A5BE3E6B65F069D,
            0xF96330ED78BE6F10, 0xEEE60DE77A076A15, 0x2B4BEE4AA08B9BD0, 0x6A56A63EC7B8894E,
            0x02121359BA34FEF4, 0x4CBF99F8283703FC, 0x398071350CAF30C8, 0xD0A77A89F017687A,
            0xF1C1A9EB9E423569, 0x8C7976282DEE8199, 0x5D1737A5DD1F7ABD, 0x4F53433C09A9FA80,
            0xFA8B0C53DF7CA1D9, 0x3FD9DCBC886CCB77, 0xC040917CA91B4720, 0x7DD00142F9D1DCDF,
            0x8476FC1D4F387B58, 0x23F8E7C5F3316503, 0x032A2244E7E37339, 0x5C87A5D750F5A74B,
            0x082B4CC43698992E, 0xDF917BECB858F63C, 0x3270B8FC5BF86DDA, 0x10AE72BB29B5DD76,
            0x576AC94E7700362B, 0x1AD112DAC61EFB8F, 0x691BC30EC5FAA427, 0xFF246311CC327143,
            0x3142368E30E53206, 0x71380E31E02CA396, 0x958D5C960AAD76F1, 0xF8D6F430C16DA536,
            0xC8FFD13F1BE7E1D2, 0x7578AE66004DDBE1, 0x05833F01067BE646, 0xBB34B5AD3BFE586D,
            0x095F34C9A12B97F0, 0x247AB64525D60CA8, 0xDCDBC6F3017477D1, 0x4A2E14D4DECAD24D,
            0xBDB5E6D9BE0A1EEB, 0x2A7E70F7794301AB, 0xDEF42D8A270540FD, 0x01078EC0A34C22C1,
            0xE5DE511AF4C16387, 0x7EBB3A52BD9A330A, 0x77697857AA7D6435, 0x004E831603AE4C32,
            0xE7A21020AD78E312, 0x9D41A70C6AB420F2, 0x28E06C18EA1141E6, 0xD2B28CBD984F6B28,
            0x26B75F6C446E9D83, 0xBA47568C4D418D7F, 0xD80BADBFE6183D8E, 0x0E206D7F5F166044,
            0xE258A43911CBCA3E, 0x723A1746B21DC0BC, 0xC7CAA854F5D7CDD3, 0x7CAC32883D261D9C,
            0x7690C26423BA942C, 0x17E55524478042B8, 0xE0BE477656A2389F, 0x4D289B5E67AB2DA0,
            0x44862B9C8FBBFD31, 0xB47CC8049D141365, 0x822C1B362B91C793, 0x4EB14655FB13DFD8,
            0x1ECBBA0714E2A97B, 0x6143459D5CDE5F14, 0x53A8FBF1D5F0AC89, 0x97EA04D81C5E5B00,
            0x622181A8D4FDB3F3, 0xE9BCD341572A1208, 0x1411258643CCE58A, 0x9144C5FEA4C6E0A4,
            0x0D33D06565CF620F, 0x54A48D489F219CA1, 0xC43E5EAC6D63C821, 0xA9728B3A72770DAF,
            0xD7934E7B20DF87EF, 0xE35503B61A3E86E5, 0xCAE321FBC819D504, 0x129A50B3AC60BFA6,
            0xCD5E68EA7E9FB6C3, 0xB01C90199483B1C7, 0x3DE93CD5C295376C, 0xAED52EDF2AB9AD13,
            0x2E60F512C0A07884, 0xBC3D86A3E36210C9, 0x35269D9B163951CE, 0x0C7D6E2AD0CDB5FA,
            0x59E86297D87F5733, 0x298EF221898DB0E7, 0x55000029D1A5AA7E, 0x8BC08AE1B5061B45,
            0xC2C31C2B6C92703A, 0x94CC596BAF25EF42, 0x0A1D73DB22540456, 0x04B6A0F9D9C4179A,
            0xEFFDAFA2AE3D3C60, 0xF7C8075BB49496C4, 0x9CC5C7141D1CD4E3, 0x78BD1638218E5534,
            0xB2F11568F850246A, 0xEDFABCFA9502BC29, 0x796CE5F2DA23051B, 0xAAE128B0DC93537C,
            0x3A493DA0EE4B29AE, 0xB5DF6B2C416895D7, 0xFCABBD25122D7F37, 0x70810B58105DC4B1,
            0xE10FDD37F7882A90, 0x524DCAB5518A3F5C, 0x3C9E85878451255B, 0x4029828119BD34E2,
            0x74A05B6F5D3CECCB, 0xB610021542E13ECA, 0x0FF979D12F59E2AC, 0x6037DA27E4F9CC50,
            0x5E92975A0DF1847D, 0xD66DE190D3E623FE, 0x5032D6B87B568048, 0x9A36B7CE8235216E,
            0x80272A7A24F64B4A, 0x93EFED8B8C6916F7, 0x37DDBFF44CCE1555, 0x4B95DB5D4B99BD25,
            0x92D3FDA169812FC0, 0xFB1A4A9A90660BB6, 0x730C196946A4B9B2, 0x81E289AA7F49DA68,
            0x64669A0F83B1A05F, 0x27B3FF7D9644F48B, 0xCC6B615C8DB675B3, 0x674F20B9BCEBBE95,
            0x6F31238275655982, 0x5AE488713E45CF05, 0xBF619F9954C21157, 0xEABAC46040A8EAE9,
            0x454C6FE9F2C0C1CD, 0x419CF6496412691C, 0xD3DC3BEF265B0F70, 0x6D0E60F5C3578A9E
        };

        private static readonly ulong[] St4 =
        {
            0x5B0E608526323C55, 0x1A46C1A9FA1B59F5, 0xA9E245A17C4C8FFA, 0x65CA5159DB2955D7,
            0x05DB0A76CE35AFC2, 0x81EAC77EA9113D45, 0x528EF88AB6AC0A0D, 0xA09EA253597BE3FF,
            0x430DDFB3AC48CD56, 0xC4B3A67AF45CE46F, 0x4ECECFD8FBE2D05E, 0x3EF56F10B39935F0,
            0x0B22D6829CD619C6, 0x17FD460A74DF2069, 0x6CF8CC8E8510ED40, 0xD6C824BF3A6ECAA7,
            0x61243D581A817049, 0x048BACB6BBC163A2, 0xD9A38AC27D44CC32, 0x7FDDFF5BAAF410AB,
            0xAD6D495AA804824B, 0xE1A6A74F2D8C9F94, 0xD4F7851235DEE8E3, 0xFD4B7F886540D893,
            0x247C20042AA4BFDA, 0x096EA1C517D1327C, 0xD56966B4361A6685, 0x277DA5C31221057D,
            0x94D59893A43ACFF7, 0x64F0C51CCDC02281, 0x3D33BCC4FF6189DB, 0xE005CB184CE66AF1,
            0xFF5CCD1D1DB99BEA, 0xB0B854A7FE42980F, 0x7BD46A6A718D4B9F, 0xD10FA8CC22A5FD8C,
            0xD31484952BE4BD31, 0xC7FA975FCB243847, 0x4886ED1E5846C407, 0x28CDDB791EB70B04,
            0xC2B00BE2F573417F, 0x5C9590452180F877, 0x7A6BDDFFF370EB00, 0xCE509E38D6D9D6A4,
            0xEBEB0F00647FA702, 0x1DCC06CF76606F06, 0xE4D9F28BA286FF0A, 0xD85A305DC918C262,
            0x475B1D8732225F54, 0x2D4FB51668CCB5FE, 0xA679B9D9D72BBA20, 0x53841C0D912D43A5,
            0x3B7EAA48BF12A4E8, 0x781E0E47F22F1DDF, 0xEFF20CE60AB50973, 0x20D261D19DFFB742,
            0x16A12B03062A2E39, 0x1960EB2239650495, 0x251C16FED50EB8B8, 0x9AC0C330F826016E,
            0xED152665953E7671, 0x02D63194A6369570, 0x5074F08394B1C987, 0x70BA598C90B25CE1,
            0x794A15810B9742F6, 0x0D5925E9FCAF8C6C, 0x3067716CD868744E, 0x910AB077E8D7731B,
            0x6A61BBDB5AC42F61, 0x93513EFBF0851567, 0xF494724B9E83E9D5, 0xE887E1985C09648D,
            0x34B1D3C675370CFD, 0xDC35E433BC0D255D, 0xD0AAB84234131BE0, 0x08042A50B48B7EAF,
            0x9997C4EE44A3AB35, 0x829A7B49201799D0, 0x263B8307B7C54441, 0x752F95F4FD6A6CA6,
            0x927217402C08C6E5, 0x2A8AB754A795D9EE, 0xA442F7552F72943D, 0x2C31334E19781208,
            0x4FA98D7CEAEE6291, 0x55C3862F665DB309, 0xBD0610175D53B1F3, 0x46FE6CB840413F27,
            0x3FE03792DF0CFA59, 0xCFE700372EB85E8F, 0xA7BE29E7ADBCE118, 0xE544EE5CDE8431DD,
            0x8A781B1B41F1873E, 0xA5C94C78A0D2F0E7, 0x39412E2877B60728, 0xA1265EF3AFC9A62C,
            0xBCC2770C6A2506C5, 0x3AB66DD5DCE1CE12, 0xE65499D04A675B37, 0x7D8F523481BFD216,
            0x0F6F64FCEC15F389, 0x74EFBE618B5B13C8, 0xACDC82B714273E1D, 0xDD40BFE003199D17,
            0x37E99257E7E061F8, 0xFA52626904775AAA, 0x8BBBF63A463D56F9, 0xF0013F1543A26E64,
            0xA8307E9F879EC898, 0xCC4C27A4150177CC, 0x1B432F2CCA1D3348, 0xDE1D1F8F9F6FA013,
            0x606602A047A7DDD6, 0xD237AB64CC1CB2C7, 0x9B938E7225FCD1D3, 0xEC4E03708E0FF476,
            0xFEB2FBDA3D03C12D, 0xAE0BCED2EE43889A, 0x22CB8923EBFB4F43, 0x69360D013CF7396D,
            0x855E3602D2D4E022, 0x073805BAD01F784C, 0x33E17A133852F546, 0xDF4874058AC7B638,
            0xBA92B29C678AA14A, 0x0CE89FC76CFAADCD, 0x5F9D4E0908339E34, 0xF1AFE9291F5923B9,
            0x6E3480F60F4A265F, 0xEEBF3A2AB29B841C, 0xE21938A88F91B4AD, 0x57DFEFF845C6D3C3,
            0x2F006B0BF62CAAF2, 0x62F479EF6F75EE78, 0x11A55AD41C8916A9, 0xF229D29084FED453,
            0x42F1C27B16B000E6, 0x2B1F76749823C074, 0x4B76ECA3C2745360, 0x8C98F463B91691BD,
            0x14BCC93CF1ADE66A, 0x8885213E6D458397, 0x8E177DF0274D4711, 0xB49B73B5503F2951,
            0x10168168C3F96B6B, 0x0E3D963B63CAB0AE, 0x8DFC4B5655A1DB14, 0xF789F1356E14DE5C,
            0x683E68AF4E51DAC1, 0xC9A84F9D8D4B0FD9, 0x3691E03F52A0F9D1, 0x5ED86E46E1878E80,
            0x3C711A0E99D07150, 0x5A0865B20C4E9310, 0x56FBFC1FE4F0682E, 0xEA8D5DE3105EDF9B,
            0x71ABFDB12379187A, 0x2EB99DE1BEE77B9C, 0x21ECC0EA33CF4523, 0x59A4D7521805C7A1,
            0x3896F5EB56AE7C72, 0xAA638F3DB18F75DC, 0x9F39358DABE9808E, 0xB7DEFA91C00B72AC,
            0x6B5541FD62492D92, 0x6DC6DEE8F92E4D5B, 0x353F57ABC4BEEA7E, 0x735769D6DA5690CE,
            0x0A234AA642391484, 0xF6F9508028F80D9D, 0xB8E319A27AB3F215, 0x31AD9C1151341A4D,
            0x773C22A57BEF5805, 0x45C7561A07968633, 0xF913DA9E249DBE36, 0xDA652D9B78A64C68,
            0x4C27A97F3BC334EF, 0x76621220E66B17F4, 0x967743899ACD7D0B, 0xF3EE5BCAE0ED6782,
            0x409F753600C879FC, 0x06D09A39B5926DB6, 0x6F83AEB0317AC588, 0x01E6CA4A86381F21,
            0x66FF3462D19F3025, 0x72207C24DDFD3BFB, 0x4AF6B6D3E2ECE2EB, 0x9C994DBEC7EA08DE,
            0x49ACE597B09A8BC4, 0xB38C4766CF0797BA, 0x131B9373C57C2A75, 0xB1822CCE61931E58,
            0x9D7555B909BA1C0C, 0x127FAFDD937D11D2, 0x29DA3BADC66D92E4, 0xA2C1D57154C2ECBC,
            0x58C5134D82F6FE24, 0x1C3AE3515B62274F, 0xE907C82E01CB8126, 0xF8ED091913E37FCB,
            0x3249D8F9C80046C9, 0x80CF9BEDE388FB63, 0x1881539A116CF19E, 0x5103F3F76BD52457,
            0x15B7E6F5AE47F7A8, 0xDBD7C6DED47E9CCF, 0x44E55C410228BB1A, 0xB647D4255EDB4E99,
            0x5D11882BB8AAFC30, 0xF5098BBB29D3212A, 0x8FB5EA14E90296B3, 0x677B942157DD025A,
            0xFB58E7C0A390ACB5, 0x89D3674C83BD4A01, 0x9E2DA4DF4BF3B93B, 0xFCC41E328CAB4829,
            0x03F38C96BA582C52, 0xCAD1BDBD7FD85DB2, 0xBBB442C16082AE83, 0xB95FE86BA5DA9AB0,
            0xB22E04673771A93F, 0x845358C9493152D8, 0xBE2A488697B4541E, 0x95A2DC2DD38E6966,
            0xC02C11AC923C852B, 0x2388B1990DF2A87B, 0x7C8008FA1B4F37BE, 0x1F70D0C84D54E503,
            0x5490ADEC7ECE57D4, 0x002B3C27D9063A3A, 0x7EAEA3848030A2BF, 0xC602326DED2003C0,
            0x83A7287D69A94086, 0xC57A5FCB30F57A8A, 0xB56844E479EBE779, 0xA373B40F05DCBCE9,
            0xD71A786E88570EE2, 0x879CBACDBDE8F6A0, 0x976AD1BCC164A32F, 0xAB21E25E9666D78B,
            0x901063AAE5E5C33C, 0x9818B34448698D90, 0xE36487AE3E1E8ABB, 0xAFBDF931893BDCB4,
            0x6345A0DC5FBBD519, 0x8628FE269B9465CA, 0x1E5D01603F9C51EC, 0x4DE44006A15049B7,
            0xBF6C70E5F776CBB1, 0x411218F2EF552BED, 0xCB0C0708705A36A3, 0xE74D14754F986044,
            0xCD56D9430EA8280E, 0xC12591D7535F5065, 0xC83223F1720AEF96, 0xC3A0396F7363A51F
        };

        #endregion

        private readonly ulong[] _state = new ulong[3];
        private int _rounds, _digestLength, _byteLength, _bufferPos;
        private ulong _processedBytes;
        private byte[] _buffer;

        private static ulong ReverseBytesUInt64(ulong value)
        {
            return (value & 0x00000000000000FF) << 56 |
                   (value & 0x000000000000FF00) << 40 |
                   (value & 0x0000000000FF0000) << 24 |
                   (value & 0x00000000FF000000) << 8 |
                   (value & 0x000000FF00000000) >> 8 |
                   (value & 0x0000FF0000000000) >> 24 |
                   (value & 0x00FF000000000000) >> 40 |
                   (value & 0xFF00000000000000) >> 56;
        }

        private static ulong le2me_64(ulong value)
        {
            return !BitConverter.IsLittleEndian ? ReverseBytesUInt64(value) : value;
        }

        private void Finish()
        {
            ulong bits = _processedBytes * 8;
            int padIndex;

            if (_bufferPos < 56)
            {
                padIndex = 56 - _bufferPos;
            }
            else
            {
                padIndex = 120 - _bufferPos;
            }


            byte[] pad = new byte[padIndex + 8];
            pad[0] = 0x80;

            bits = le2me_64(bits);

            Pack.UInt64_To_LE(bits, pad, padIndex);

            padIndex += 8;

            BlockUpdate(pad, 0, padIndex);
        }

        private void ProcessBlock(ref ulong[] block)
        {
            ulong a = _state[0];
            ulong b = _state[1];
            ulong c = _state[2];

            c ^= block[0];
            a -= St1[(byte) c] ^ St2[(byte) (c >> 16)] ^ St3[(byte) (c >> 32)] ^ St4[(byte) (c >> 48)];
            b += St4[(byte) (c >> 8)] ^ St3[(byte) (c >> 24)] ^ St2[(byte) (c >> 40)] ^ St1[(byte) (c >> 56)];
            b *= 5;

            a ^= block[1];
            b -= St1[(byte) a] ^ St2[(byte) (a >> 16)] ^ St3[(byte) (a >> 32)] ^ St4[(byte) (a >> 48)];
            c += St4[(byte) (a >> 8)] ^ St3[(byte) (a >> 24)] ^ St2[(byte) (a >> 40)] ^ St1[(byte) (a >> 56)];
            c *= 5;

            b ^= block[2];
            c -= St1[(byte) b] ^ St2[(byte) (b >> 16)] ^ St3[(byte) (b >> 32)] ^ St4[(byte) (b >> 48)];
            a += St4[(byte) (b >> 8)] ^ St3[(byte) (b >> 24)] ^ St2[(byte) (b >> 40)] ^ St1[(byte) (b >> 56)];
            a *= 5;

            c ^= block[3];
            a -= St1[(byte) c] ^ St2[(byte) (c >> 16)] ^ St3[(byte) (c >> 32)] ^ St4[(byte) (c >> 48)];
            b += St4[(byte) (c >> 8)] ^ St3[(byte) (c >> 24)] ^ St2[(byte) (c >> 40)] ^ St1[(byte) (c >> 56)];
            b *= 5;

            a ^= block[4];
            b -= St1[(byte) a] ^ St2[(byte) (a >> 16)] ^ St3[(byte) (a >> 32)] ^ St4[(byte) (a >> 48)];
            c += St4[(byte) (a >> 8)] ^ St3[(byte) (a >> 24)] ^ St2[(byte) (a >> 40)] ^ St1[(byte) (a >> 56)];
            c *= 5;

            b ^= block[5];
            c -= St1[(byte) b] ^ St2[(byte) (b >> 16)] ^ St3[(byte) (b >> 32)] ^ St4[(byte) (b >> 48)];
            a += St4[(byte) (b >> 8)] ^ St3[(byte) (b >> 24)] ^ St2[(byte) (b >> 40)] ^ St1[(byte) (b >> 56)];
            a *= 5;

            c ^= block[6];
            a -= St1[(byte) c] ^ St2[(byte) (c >> 16)] ^ St3[(byte) (c >> 32)] ^ St4[(byte) (c >> 48)];
            b += St4[(byte) (c >> 8)] ^ St3[(byte) (c >> 24)] ^ St2[(byte) (c >> 40)] ^ St1[(byte) (c >> 56)];
            b *= 5;

            a ^= block[7];
            b -= St1[(byte) a] ^ St2[(byte) (a >> 16)] ^ St3[(byte) (a >> 32)] ^ St4[(byte) (a >> 48)];
            c += St4[(byte) (a >> 8)] ^ St3[(byte) (a >> 24)] ^ St2[(byte) (a >> 40)] ^ St1[(byte) (a >> 56)];
            c *= 5;

            block[0] -= block[7] ^ C1;
            block[1] ^= block[0];
            block[2] += block[1];
            block[3] -= block[2] ^ (~block[1] << 19);
            block[4] ^= block[3];
            block[5] += block[4];
            block[6] -= block[5] ^ (~block[4] >> 23);
            block[7] ^= block[6];
            block[0] += block[7];
            block[1] -= block[0] ^ (~block[7] << 19);
            block[2] ^= block[1];
            block[3] += block[2];
            block[4] -= block[3] ^ (~block[2] >> 23);
            block[5] ^= block[4];
            block[6] += block[5];
            block[7] -= block[6] ^ C2;

            b ^= block[0];
            c -= St1[(byte) b] ^ St2[(byte) (b >> 16)] ^ St3[(byte) (b >> 32)] ^ St4[(byte) (b >> 48)];
            a += St4[(byte) (b >> 8)] ^ St3[(byte) (b >> 24)] ^ St2[(byte) (b >> 40)] ^ St1[(byte) (b >> 56)];
            a *= 7;

            c ^= block[1];
            a -= St1[(byte) c] ^ St2[(byte) (c >> 16)] ^ St3[(byte) (c >> 32)] ^ St4[(byte) (c >> 48)];
            b += St4[(byte) (c >> 8)] ^ St3[(byte) (c >> 24)] ^ St2[(byte) (c >> 40)] ^ St1[(byte) (c >> 56)];
            b *= 7;

            a ^= block[2];
            b -= St1[(byte) a] ^ St2[(byte) (a >> 16)] ^ St3[(byte) (a >> 32)] ^ St4[(byte) (a >> 48)];
            c += St4[(byte) (a >> 8)] ^ St3[(byte) (a >> 24)] ^ St2[(byte) (a >> 40)] ^ St1[(byte) (a >> 56)];
            c *= 7;

            b ^= block[3];
            c -= St1[(byte) b] ^ St2[(byte) (b >> 16)] ^ St3[(byte) (b >> 32)] ^ St4[(byte) (b >> 48)];
            a += St4[(byte) (b >> 8)] ^ St3[(byte) (b >> 24)] ^ St2[(byte) (b >> 40)] ^ St1[(byte) (b >> 56)];
            a *= 7;

            c ^= block[4];
            a -= St1[(byte) c] ^ St2[(byte) (c >> 16)] ^ St3[(byte) (c >> 32)] ^ St4[(byte) (c >> 48)];
            b += St4[(byte) (c >> 8)] ^ St3[(byte) (c >> 24)] ^ St2[(byte) (c >> 40)] ^ St1[(byte) (c >> 56)];
            b *= 7;

            a ^= block[5];
            b -= St1[(byte) a] ^ St2[(byte) (a >> 16)] ^ St3[(byte) (a >> 32)] ^ St4[(byte) (a >> 48)];
            c += St4[(byte) (a >> 8)] ^ St3[(byte) (a >> 24)] ^ St2[(byte) (a >> 40)] ^ St1[(byte) (a >> 56)];
            c *= 7;

            b ^= block[6];
            c -= St1[(byte) b] ^ St2[(byte) (b >> 16)] ^ St3[(byte) (b >> 32)] ^ St4[(byte) (b >> 48)];
            a += St4[(byte) (b >> 8)] ^ St3[(byte) (b >> 24)] ^ St2[(byte) (b >> 40)] ^ St1[(byte) (b >> 56)];
            a *= 7;

            c ^= block[7];
            a -= St1[(byte) c] ^ St2[(byte) (c >> 16)] ^ St3[(byte) (c >> 32)] ^ St4[(byte) (c >> 48)];
            b += St4[(byte) (c >> 8)] ^ St3[(byte) (c >> 24)] ^ St2[(byte) (c >> 40)] ^ St1[(byte) (c >> 56)];
            b *= 7;

            block[0] -= block[7] ^ C1;
            block[1] ^= block[0];
            block[2] += block[1];
            block[3] -= block[2] ^ (~block[1] << 19);
            block[4] ^= block[3];
            block[5] += block[4];
            block[6] -= block[5] ^ (~block[4] >> 23);
            block[7] ^= block[6];
            block[0] += block[7];
            block[1] -= block[0] ^ (~block[7] << 19);
            block[2] ^= block[1];
            block[3] += block[2];
            block[4] -= block[3] ^ (~block[2] >> 23);
            block[5] ^= block[4];
            block[6] += block[5];
            block[7] -= block[6] ^ C2;

            a ^= block[0];
            b -= St1[(byte) a] ^ St2[(byte) (a >> 16)] ^ St3[(byte) (a >> 32)] ^ St4[(byte) (a >> 48)];
            c += St4[(byte) (a >> 8)] ^ St3[(byte) (a >> 24)] ^ St2[(byte) (a >> 40)] ^ St1[(byte) (a >> 56)];
            c *= 9;

            b ^= block[1];
            c -= St1[(byte) b] ^ St2[(byte) (b >> 16)] ^ St3[(byte) (b >> 32)] ^ St4[(byte) (b >> 48)];
            a += St4[(byte) (b >> 8)] ^ St3[(byte) (b >> 24)] ^ St2[(byte) (b >> 40)] ^ St1[(byte) (b >> 56)];
            a *= 9;

            c ^= block[2];
            a -= St1[(byte) c] ^ St2[(byte) (c >> 16)] ^ St3[(byte) (c >> 32)] ^ St4[(byte) (c >> 48)];
            b += St4[(byte) (c >> 8)] ^ St3[(byte) (c >> 24)] ^ St2[(byte) (c >> 40)] ^ St1[(byte) (c >> 56)];
            b *= 9;

            a ^= block[3];
            b -= St1[(byte) a] ^ St2[(byte) (a >> 16)] ^ St3[(byte) (a >> 32)] ^ St4[(byte) (a >> 48)];
            c += St4[(byte) (a >> 8)] ^ St3[(byte) (a >> 24)] ^ St2[(byte) (a >> 40)] ^ St1[(byte) (a >> 56)];
            c *= 9;

            b ^= block[4];
            c -= St1[(byte) b] ^ St2[(byte) (b >> 16)] ^ St3[(byte) (b >> 32)] ^ St4[(byte) (b >> 48)];
            a += St4[(byte) (b >> 8)] ^ St3[(byte) (b >> 24)] ^ St2[(byte) (b >> 40)] ^ St1[(byte) (b >> 56)];
            a *= 9;

            c ^= block[5];
            a -= St1[(byte) c] ^ St2[(byte) (c >> 16)] ^ St3[(byte) (c >> 32)] ^ St4[(byte) (c >> 48)];
            b += St4[(byte) (c >> 8)] ^ St3[(byte) (c >> 24)] ^ St2[(byte) (c >> 40)] ^ St1[(byte) (c >> 56)];
            b *= 9;

            a ^= block[6];
            b -= St1[(byte) a] ^ St2[(byte) (a >> 16)] ^ St3[(byte) (a >> 32)] ^ St4[(byte) (a >> 48)];
            c += St4[(byte) (a >> 8)] ^ St3[(byte) (a >> 24)] ^ St2[(byte) (a >> 40)] ^ St1[(byte) (a >> 56)];
            c *= 9;

            b ^= block[7];
            c -= St1[(byte) b] ^ St2[(byte) (b >> 16)] ^ St3[(byte) (b >> 32)] ^ St4[(byte) (b >> 48)];
            a += St4[(byte) (b >> 8)] ^ St3[(byte) (b >> 24)] ^ St2[(byte) (b >> 40)] ^ St1[(byte) (b >> 56)];
            a *= 9;

            int rounds = 3;
            while (rounds < _rounds)
            {
                block[0] = block[0] - (block[7] ^ C1);
                block[1] = block[1] ^ block[0];
                block[2] = block[2] + block[1];
                block[3] = block[3] - (block[2] ^ (~block[1] << 19));
                block[4] = block[4] ^ block[3];
                block[5] = block[5] + block[4];
                block[6] = block[6] - (block[5] ^ (~block[4] >> 23));
                block[7] = block[7] ^ block[6];
                block[0] = block[0] + block[7];
                block[1] = block[1] - (block[0] ^ (~block[7] << 19));
                block[2] = block[2] ^ block[1];
                block[3] = block[3] + block[2];
                block[4] = block[4] - (block[3] ^ (~block[2] >> 23));
                block[5] = block[5] ^ block[4];
                block[6] = block[6] + block[5];
                block[7] = block[7] - (block[6] ^ C2);

                c = c ^ block[0];
                a = a - (St1[(byte) c] ^ St2[(byte) (c >> 16)] ^ St3[(byte) (c >> 32)
                         ] ^ St4[(byte) (c >> 48)]);
                b = b + (St4[(byte) (c >> 8) & 0xFF] ^ St3[(byte) (c >> 24)
                         ] ^ St2[(byte) (c >> 40)] ^ St1[(byte) (c >> 56)]);
                b = b * 9;

                a = a ^ block[1];
                b = b - (St1[(byte) a] ^ St2[(byte) (a >> 16)] ^ St3[(byte) (a >> 32)
                         ] ^ St4[(byte) (a >> 48)]);
                c = c + (St4[(byte) (a >> 8)] ^ St3[(byte) (a >> 24)] ^ St2
                             [(byte) (a >> 40)] ^ St1[(byte) (a >> 56)]);
                c = c * 9;

                b = b ^ block[2];
                c = c - (St1[(byte) b] ^ St2[(byte) (b >> 16)] ^ St3[(byte) (b >> 32)
                         ] ^ St4[(byte) (b >> 48)]);
                a = a + (St4[(byte) (b >> 8)] ^ St3[(byte) (b >> 24)] ^ St2
                             [(byte) (b >> 40)] ^ St1[(byte) (b >> 56)]);
                a = a * 9;

                c = c ^ block[3];
                a = a - (St1[(byte) c] ^ St2[(byte) (c >> 16)] ^ St3[(byte) (c >> 32)
                         ] ^ St4[(byte) (c >> 48)]);
                b = b + (St4[(byte) (c >> 8) & 0xFF] ^ St3[(byte) (c >> 24)
                         ] ^ St2[(byte) (c >> 40)] ^ St1[(byte) (c >> 56)]);
                b = b * 9;

                a = a ^ block[4];
                b = b - (St1[(byte) a] ^ St2[(byte) (a >> 16)] ^ St3[(byte) (a >> 32)
                         ] ^ St4[(byte) (a >> 48)]);
                c = c + (St4[(byte) (a >> 8)] ^ St3[(byte) (a >> 24)] ^ St2
                             [(byte) (a >> 40)] ^ St1[(byte) (a >> 56)]);
                c = c * 9;

                b = b ^ block[5];
                c = c - (St1[(byte) b] ^ St2[(byte) (b >> 16)] ^ St3[(byte) (b >> 32)
                         ] ^ St4[(byte) (b >> 48)]);
                a = a + (St4[(byte) (b >> 8)] ^ St3[(byte) (b >> 24)] ^ St2
                             [(byte) (b >> 40)] ^ St1[(byte) (b >> 56)]);
                a = a * 9;

                c = c ^ block[6];
                a = a - (St1[(byte) c] ^ St2[(byte) (c >> 16)] ^ St3[(byte) (c >> 32)
                         ] ^ St4[(byte) (c >> 48)]);
                b = b + (St4[(byte) (c >> 8) & 0xFF] ^ St3[(byte) (c >> 24)
                         ] ^ St2[(byte) (c >> 40)] ^ St1[(byte) (c >> 56)]);
                b = b * 9;

                a = a ^ block[7];
                b = b - (St1[(byte) a] ^ St2[(byte) (a >> 16)] ^ St3[(byte) (a >> 32)
                         ] ^ St4[(byte) (a >> 48)]);
                c = c + (St4[(byte) (a >> 8)] ^ St3[(byte) (a >> 24)] ^ St2
                             [(byte) (a >> 40)] ^ St1[(byte) (a >> 56)]);
                c = c * 9;

                ulong tempA = a;
                a = c;
                c = b;
                b = tempA;

                rounds++;
            }


            _state[0] ^= a;
            _state[1] = b - _state[1];
            _state[2] += c;


            Array.Clear(block, 0, block.Length);
        }

        // this takes a buffer of information and fills the block
        private void ProcessFilledBuffer()
        {
            ulong[] work = new ulong[8];

            Pack.LE_To_UInt64(_buffer, 0, work);

            ProcessBlock(ref work);
            _bufferPos = 0;
            Array.Clear(_buffer, 0, _buffer.Length);
        }

        /**
        * Standard constructor
        */
        protected Tiger2Digest(int rounds, int digestLength, int byteLength)
        {
            _rounds = rounds;
            _digestLength = digestLength;
            _byteLength = byteLength;
            _buffer = new byte[GetByteLength()];
            Reset();
        }

        /**
        * Copy constructor. This will copy the state of the provided
        * message digest.
        */
        public Tiger2Digest(Tiger2Digest t)
        {
            Reset(t);
        }

        public int GetDigestSize()
        {
            return _digestLength;
        }

        public int GetByteLength()
        {
            return _byteLength;
        }

        public void Update(byte input)
        {
            _buffer[_bufferPos] = input;

            _processedBytes++;

            _bufferPos++;

            if (_bufferPos == _buffer.Length)
            {
                ProcessFilledBuffer();
            }
        }

        public void BlockUpdate(byte[] input, int inOff, int length)
        {
            while (length > 0)
            {
                Update(input[inOff]);
                ++inOff;
                --length;
            }
        }

        public int DoFinal(byte[] output, int outOff)
        {
            Finish();

            Pack.UInt64_To_LE(_state, output, outOff);

            Reset();

            return GetDigestSize();
        }

        public void Reset()
        {
            _state[0] = 0x0123456789ABCDEF;
            _state[1] = 0xFEDCBA9876543210;
            _state[2] = 0xF096A5B4C3B2E187;
            Array.Clear(_buffer, 0, _buffer.Length);
            _bufferPos = 0;
            _processedBytes = 0;
        }

        public string AlgorithmName
        {
            get { return "Tiger2_" + _rounds + "_" + (GetDigestSize() * 8); }
        }

        public IMemoable Copy()
        {
            return new Tiger2Digest(this);
        }

        public void Reset(IMemoable other)
        {
            Tiger2Digest originalDigest = (Tiger2Digest) other;

            Array.Copy(originalDigest._state, 0, _state, 0, _state.Length);
            _buffer = new byte[originalDigest._buffer.Length];
            Array.Copy(originalDigest._buffer, 0, _buffer, 0, _buffer.Length);

            _rounds = originalDigest._rounds;
            _digestLength = originalDigest._digestLength;
            _byteLength = originalDigest._byteLength;
            _bufferPos = originalDigest._bufferPos;
            _processedBytes = originalDigest._processedBytes;
        }
    }

    public class Tiger2_5_192Digest : Tiger2Digest
    {
        /**
        * Standard Tiger2_5_192 constructor
        */
        public Tiger2_5_192Digest()
            : base(5, 24, 64)
        {
        }
    }
}