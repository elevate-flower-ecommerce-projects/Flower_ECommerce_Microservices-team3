using Address___Store_Coverage_Service.Entities;
using Microsoft.EntityFrameworkCore;

namespace Address___Store_Coverage_Service.Persistence.Seeding
{
    public static class CityAreaSeeder
    {
        private const string SeedActor = "system:seed";

        public static async Task SeedAsync(
            FlowersAddressStoreCoverageDbContext context,
            CancellationToken cancellationToken = default)
        {
            if (await context.Areas.IgnoreQueryFilters().AnyAsync(cancellationToken))
            {
                return;
            }

            var seededAt = DateTime.UtcNow;

            var areas = new List<Area>
            {
                // 1. Cairo
                new()
                {
                    Id = new Guid("a1000000-0000-0000-0000-000000000001"),
                    Name = "Cairo",
                    CreatedAt = seededAt,
                    CreatedBy = SeedActor,
                    Cities = new List<City>
                    {
                        NewCity("c1000000-0000-0000-0000-000000000001", "15 May", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000002", "Al Azbakeyah", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000003", "Al Basatin", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000004", "Tebin", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000005", "El-Khalifa", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000006", "El darrasa", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000007", "Aldarb Alahmar", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000008", "Zawya al-Hamra", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000009", "El-Zaytoun", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000010", "Sahel", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000011", "El Salam", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000012", "Sayeda Zeinab", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000013", "El Sharabeya", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000014", "Shorouk", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000015", "El Daher", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000016", "Ataba", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000017", "New Cairo", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000018", "El Marg", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000019", "Ezbet el Nakhl", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000020", "Matareya", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000021", "Maadi", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000022", "Maasara", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000023", "Mokattam", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000024", "Manyal", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000025", "Mosky", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000026", "Nozha", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000027", "Waily", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000028", "Bab al-Shereia", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000029", "Bolaq", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000030", "Garden City", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000031", "Hadayek El-Kobba", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000032", "Helwan", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000033", "Dar Al Salam", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000034", "Shubra", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000035", "Tura", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000036", "Abdeen", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000037", "Abaseya", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000038", "Ain Shams", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000039", "Nasr City", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000040", "New Heliopolis", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000041", "Masr Al Qadima", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000042", "Mansheya Nasir", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000043", "Badr City", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000044", "Obour City", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000045", "Cairo Downtown", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000046", "Zamalek", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000047", "Kasr El Nile", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000048", "Rehab", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000049", "Katameya", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000050", "Madinty", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000051", "Rod Alfarag", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000052", "Sheraton", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000053", "El-Gamaleya", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000054", "10th of Ramadan City", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000055", "Helmeyat Alzaytoun", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000056", "New Nozha", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000057", "Capital New", seededAt),
                    }
                },

                // 2. Giza
                new()
                {
                    Id = new Guid("a1000000-0000-0000-0000-000000000002"),
                    Name = "Giza",
                    CreatedAt = seededAt,
                    CreatedBy = SeedActor,
                    Cities = new List<City>
                    {
                        NewCity("c1000000-0000-0000-0000-000000000058", "Giza", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000059", "Sixth of October", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000060", "Cheikh Zayed", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000061", "Hawamdiyah", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000062", "Al Badrasheen", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000063", "Saf", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000064", "Atfih", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000065", "Al Ayat", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000066", "Al-Bawaiti", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000067", "ManshiyetAl Qanater", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000068", "Oaseem", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000069", "Kerdasa", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000070", "Abu Nomros", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000071", "Kafr Ghati", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000072", "Manshiyet Al Bakari", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000073", "Dokki", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000074", "Agouza", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000075", "Haram", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000076", "Warraq", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000077", "Imbaba", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000078", "Boulaq Dakrour", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000079", "Al Wahat Al Baharia", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000080", "Omraneya", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000081", "Moneeb", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000082", "Bin Alsarayat", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000083", "Kit Kat", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000084", "Mohandessin", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000085", "Faisal", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000086", "Abu Rawash", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000087", "Hadayek Alahram", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000088", "Haraneya", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000089", "Hadayek October", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000090", "Saft Allaban", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000091", "Smart Village", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000092", "Ard Ellwaa", seededAt),
                    }
                },

                // 3. Alexandria
                new()
                {
                    Id = new Guid("a1000000-0000-0000-0000-000000000003"),
                    Name = "Alexandria",
                    CreatedAt = seededAt,
                    CreatedBy = SeedActor,
                    Cities = new List<City>
                    {
                        NewCity("c1000000-0000-0000-0000-000000000093", "Abu Qir", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000094", "Al Ibrahimeyah", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000095", "Azarita", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000096", "Anfoushi", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000097", "Dekheila", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000098", "El Soyof", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000099", "Ameria", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000100", "El Labban", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000101", "Al Mafrouza", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000102", "El Montaza", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000103", "Mansheya", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000104", "Naseria", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000105", "Ambrozo", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000106", "Bab Sharq", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000107", "Bourj Alarab", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000108", "Stanley", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000109", "Smouha", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000110", "Sidi Bishr", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000111", "Shads", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000112", "Gheet Alenab", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000113", "Fleming", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000114", "Victoria", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000115", "Camp Shizar", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000116", "Karmooz", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000117", "Mahta Alraml", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000118", "Mina El-Basal", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000119", "Asafra", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000120", "Agamy", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000121", "Bakos", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000122", "Boulkly", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000123", "Cleopatra", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000124", "Glim", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000125", "Al Mamurah", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000126", "Al Mandara", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000127", "Moharam Bek", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000128", "Elshatby", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000129", "Sidi Gaber", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000130", "North Coast/sahel", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000131", "Alhadra", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000132", "Alattarin", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000133", "Sidi Kerir", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000134", "Elgomrok", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000135", "Al Max", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000136", "Marina", seededAt),
                    }
                },

                // 4. Dakahlia
                new()
                {
                    Id = new Guid("a1000000-0000-0000-0000-000000000004"),
                    Name = "Dakahlia",
                    CreatedAt = seededAt,
                    CreatedBy = SeedActor,
                    Cities = new List<City>
                    {
                        NewCity("c1000000-0000-0000-0000-000000000137", "Mansoura", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000138", "Talkha", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000139", "Mitt Ghamr", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000140", "Dekernes", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000141", "Aga", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000142", "Menia El Nasr", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000143", "Sinbillawin", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000144", "El Kurdi", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000145", "Bani Ubaid", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000146", "Al Manzala", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000147", "tami al'amdid", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000148", "aljamalia", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000149", "Sherbin", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000150", "Mataria", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000151", "Belqas", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000152", "Meet Salsil", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000153", "Gamasa", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000154", "Mahalat Damana", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000155", "Nabroh", seededAt),
                    }
                },

                // 5. Red Sea
                new()
                {
                    Id = new Guid("a1000000-0000-0000-0000-000000000005"),
                    Name = "Red Sea",
                    CreatedAt = seededAt,
                    CreatedBy = SeedActor,
                    Cities = new List<City>
                    {
                        NewCity("c1000000-0000-0000-0000-000000000156", "Hurghada", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000157", "Ras Ghareb", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000158", "Safaga", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000159", "El Qusiar", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000160", "Marsa Alam", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000161", "Shalatin", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000162", "Halaib", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000163", "Aldahar", seededAt),
                    }
                },

                // 6. Beheira
                new()
                {
                    Id = new Guid("a1000000-0000-0000-0000-000000000006"),
                    Name = "Beheira",
                    CreatedAt = seededAt,
                    CreatedBy = SeedActor,
                    Cities = new List<City>
                    {
                        NewCity("c1000000-0000-0000-0000-000000000164", "Damanhour", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000165", "Kafr El Dawar", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000166", "Rashid", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000167", "Edco", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000168", "Abu al-Matamir", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000169", "Abu Homs", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000170", "Delengat", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000171", "Mahmoudiyah", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000172", "Rahmaniyah", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000173", "Itai Baroud", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000174", "Housh Eissa", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000175", "Shubrakhit", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000176", "Kom Hamada", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000177", "Badr", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000178", "Wadi Natrun", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000179", "New Nubaria", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000180", "Alnoubareya", seededAt),
                    }
                },

                // 7. Fayoum
                new()
                {
                    Id = new Guid("a1000000-0000-0000-0000-000000000007"),
                    Name = "Fayoum",
                    CreatedAt = seededAt,
                    CreatedBy = SeedActor,
                    Cities = new List<City>
                    {
                        NewCity("c1000000-0000-0000-0000-000000000181", "Fayoum", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000182", "Fayoum El Gedida", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000183", "Tamiya", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000184", "Snores", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000185", "Etsa", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000186", "Epschway", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000187", "Yusuf El Sediaq", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000188", "Hadqa", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000189", "Atsa", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000190", "Algamaa", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000191", "Sayala", seededAt),
                    }
                },

                // 8. Gharbiya
                new()
                {
                    Id = new Guid("a1000000-0000-0000-0000-000000000008"),
                    Name = "Gharbiya",
                    CreatedAt = seededAt,
                    CreatedBy = SeedActor,
                    Cities = new List<City>
                    {
                        NewCity("c1000000-0000-0000-0000-000000000192", "Tanta", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000193", "Al Mahalla Al Kobra", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000194", "Kafr El Zayat", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000195", "Zefta", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000196", "El Santa", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000197", "Qutour", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000198", "Basion", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000199", "Samannoud", seededAt),
                    }
                },

                // 9. Ismailia
                new()
                {
                    Id = new Guid("a1000000-0000-0000-0000-000000000009"),
                    Name = "Ismailia",
                    CreatedAt = seededAt,
                    CreatedBy = SeedActor,
                    Cities = new List<City>
                    {
                        NewCity("c1000000-0000-0000-0000-000000000200", "Ismailia", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000201", "Fayed", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000202", "Qantara Sharq", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000203", "Qantara Gharb", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000204", "El Tal El Kabier", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000205", "Abu Sawir", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000206", "Kasasien El Gedida", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000207", "Nefesha", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000208", "Sheikh Zayed", seededAt),
                    }
                },

                // 10. Menofia
                new()
                {
                    Id = new Guid("a1000000-0000-0000-0000-000000000010"),
                    Name = "Menofia",
                    CreatedAt = seededAt,
                    CreatedBy = SeedActor,
                    Cities = new List<City>
                    {
                        NewCity("c1000000-0000-0000-0000-000000000209", "Shbeen El Koom", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000210", "Sadat City", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000211", "Menouf", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000212", "Sars El-Layan", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000213", "Ashmon", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000214", "Al Bagor", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000215", "Quesna", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000216", "Berkat El Saba", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000217", "Tala", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000218", "Al Shohada", seededAt),
                    }
                },

                // 11. Minya
                new()
                {
                    Id = new Guid("a1000000-0000-0000-0000-000000000011"),
                    Name = "Minya",
                    CreatedAt = seededAt,
                    CreatedBy = SeedActor,
                    Cities = new List<City>
                    {
                        NewCity("c1000000-0000-0000-0000-000000000219", "Minya", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000220", "Minya El Gedida", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000221", "El Adwa", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000222", "Magagha", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000223", "Bani Mazar", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000224", "Mattay", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000225", "Samalut", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000226", "Madinat El Fekria", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000227", "Meloy", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000228", "Deir Mawas", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000229", "Abu Qurqas", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000230", "Ard Sultan", seededAt),
                    }
                },

                // 12. Qaliubiya
                new()
                {
                    Id = new Guid("a1000000-0000-0000-0000-000000000012"),
                    Name = "Qaliubiya",
                    CreatedAt = seededAt,
                    CreatedBy = SeedActor,
                    Cities = new List<City>
                    {
                        NewCity("c1000000-0000-0000-0000-000000000231", "Banha", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000232", "Qalyub", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000233", "Shubra Al Khaimah", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000234", "Al Qanater Charity", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000235", "Khanka", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000236", "Kafr Shukr", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000237", "Tukh", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000238", "Qaha", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000239", "Obour", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000240", "Khosous", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000241", "Shibin Al Qanater", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000242", "Mostorod", seededAt),
                    }
                },

                // 13. New Valley
                new()
                {
                    Id = new Guid("a1000000-0000-0000-0000-000000000013"),
                    Name = "New Valley",
                    CreatedAt = seededAt,
                    CreatedBy = SeedActor,
                    Cities = new List<City>
                    {
                        NewCity("c1000000-0000-0000-0000-000000000243", "El Kharga", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000244", "Paris", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000245", "Mout", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000246", "Farafra", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000247", "Balat", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000248", "Dakhla", seededAt),
                    }
                },

                // 14. Suez
                new()
                {
                    Id = new Guid("a1000000-0000-0000-0000-000000000014"),
                    Name = "Suez",
                    CreatedAt = seededAt,
                    CreatedBy = SeedActor,
                    Cities = new List<City>
                    {
                        NewCity("c1000000-0000-0000-0000-000000000249", "Suez", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000250", "Alganayen", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000251", "Ataqah", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000252", "Ain Sokhna", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000253", "Faysal", seededAt),
                    }
                },

                // 15. Aswan
                new()
                {
                    Id = new Guid("a1000000-0000-0000-0000-000000000015"),
                    Name = "Aswan",
                    CreatedAt = seededAt,
                    CreatedBy = SeedActor,
                    Cities = new List<City>
                    {
                        NewCity("c1000000-0000-0000-0000-000000000254", "Aswan", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000255", "Aswan El Gedida", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000256", "Drau", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000257", "Kom Ombo", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000258", "Nasr Al Nuba", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000259", "Kalabsha", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000260", "Edfu", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000261", "Al-Radisiyah", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000262", "Al Basilia", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000263", "Al Sibaeia", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000264", "Abo Simbl Al Siyahia", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000265", "Marsa Alam", seededAt),
                    }
                },

                // 16. Assiut
                new()
                {
                    Id = new Guid("a1000000-0000-0000-0000-000000000016"),
                    Name = "Assiut",
                    CreatedAt = seededAt,
                    CreatedBy = SeedActor,
                    Cities = new List<City>
                    {
                        NewCity("c1000000-0000-0000-0000-000000000266", "Assiut", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000267", "Assiut El Gedida", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000268", "Dayrout", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000269", "Manfalut", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000270", "Qusiya", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000271", "Abnoub", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000272", "Abu Tig", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000273", "El Ghanaim", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000274", "Sahel Selim", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000275", "El Badari", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000276", "Sidfa", seededAt),
                    }
                },

                // 17. Beni Suef
                new()
                {
                    Id = new Guid("a1000000-0000-0000-0000-000000000017"),
                    Name = "Beni Suef",
                    CreatedAt = seededAt,
                    CreatedBy = SeedActor,
                    Cities = new List<City>
                    {
                        NewCity("c1000000-0000-0000-0000-000000000277", "Bani Sweif", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000278", "Beni Suef El Gedida", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000279", "Al Wasta", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000280", "Naser", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000281", "Ehnasia", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000282", "beba", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000283", "Fashn", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000284", "Somasta", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000285", "Alabbaseri", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000286", "Mokbel", seededAt),
                    }
                },

                // 18. Port Said
                new()
                {
                    Id = new Guid("a1000000-0000-0000-0000-000000000018"),
                    Name = "Port Said",
                    CreatedAt = seededAt,
                    CreatedBy = SeedActor,
                    Cities = new List<City>
                    {
                        NewCity("c1000000-0000-0000-0000-000000000287", "PorSaid", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000288", "Port Fouad", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000289", "Alarab", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000290", "Zohour", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000291", "Alsharq", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000292", "Aldawahi", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000293", "Almanakh", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000294", "Mubarak", seededAt),
                    }
                },

                // 19. Damietta
                new()
                {
                    Id = new Guid("a1000000-0000-0000-0000-000000000019"),
                    Name = "Damietta",
                    CreatedAt = seededAt,
                    CreatedBy = SeedActor,
                    Cities = new List<City>
                    {
                        NewCity("c1000000-0000-0000-0000-000000000295", "Damietta", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000296", "New Damietta", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000297", "Ras El Bar", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000298", "Faraskour", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000299", "Zarqa", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000300", "alsaru", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000301", "alruwda", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000302", "Kafr El-Batikh", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000303", "Azbet Al Burg", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000304", "Meet Abou Ghalib", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000305", "Kafr Saad", seededAt),
                    }
                },

                // 20. Sharkia
                new()
                {
                    Id = new Guid("a1000000-0000-0000-0000-000000000020"),
                    Name = "Sharkia",
                    CreatedAt = seededAt,
                    CreatedBy = SeedActor,
                    Cities = new List<City>
                    {
                        NewCity("c1000000-0000-0000-0000-000000000306", "Zagazig", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000307", "Al Ashr Men Ramadan", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000308", "Minya Al Qamh", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000309", "Belbeis", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000310", "Mashtoul El Souq", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000311", "Qenaiat", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000312", "Abu Hammad", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000313", "El Qurain", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000314", "Hehia", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000315", "Abu Kabir", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000316", "Faccus", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000317", "El Salihia El Gedida", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000318", "Al Ibrahimiyah", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000319", "Deirb Negm", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000320", "Kafr Saqr", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000321", "Awlad Saqr", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000322", "Husseiniya", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000323", "san alhajar alqablia", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000324", "Manshayat Abu Omar", seededAt),
                    }
                },

                // 21. South Sinai
                new()
                {
                    Id = new Guid("a1000000-0000-0000-0000-000000000021"),
                    Name = "South Sinai",
                    CreatedAt = seededAt,
                    CreatedBy = SeedActor,
                    Cities = new List<City>
                    {
                        NewCity("c1000000-0000-0000-0000-000000000325", "Al Toor", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000326", "Sharm El-Shaikh", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000327", "Dahab", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000328", "Nuweiba", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000329", "Taba", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000330", "Saint Catherine", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000331", "Abu Redis", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000332", "Abu Zenaima", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000333", "Ras Sidr", seededAt),
                    }
                },

                // 22. Kafr Al sheikh
                new()
                {
                    Id = new Guid("a1000000-0000-0000-0000-000000000022"),
                    Name = "Kafr Al sheikh",
                    CreatedAt = seededAt,
                    CreatedBy = SeedActor,
                    Cities = new List<City>
                    {
                        NewCity("c1000000-0000-0000-0000-000000000334", "Kafr El Sheikh", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000335", "Kafr El Sheikh Downtown", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000336", "Desouq", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000337", "Fooh", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000338", "Metobas", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000339", "Burg Al Burullus", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000340", "Baltim", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000341", "Masief Baltim", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000342", "Hamol", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000343", "Bella", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000344", "Riyadh", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000345", "Sidi Salm", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000346", "Qellen", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000347", "Sidi Ghazi", seededAt),
                    }
                },

                // 23. Matrouh
                new()
                {
                    Id = new Guid("a1000000-0000-0000-0000-000000000023"),
                    Name = "Matrouh",
                    CreatedAt = seededAt,
                    CreatedBy = SeedActor,
                    Cities = new List<City>
                    {
                        NewCity("c1000000-0000-0000-0000-000000000348", "Marsa Matrouh", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000349", "El Hamam", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000350", "Alamein", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000351", "Dabaa", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000352", "Al-Nagila", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000353", "Sidi Brani", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000354", "Salloum", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000355", "Siwa", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000356", "Marina", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000357", "North Coast", seededAt),
                    }
                },

                // 24. Luxor
                new()
                {
                    Id = new Guid("a1000000-0000-0000-0000-000000000024"),
                    Name = "Luxor",
                    CreatedAt = seededAt,
                    CreatedBy = SeedActor,
                    Cities = new List<City>
                    {
                        NewCity("c1000000-0000-0000-0000-000000000358", "Luxor", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000359", "New Luxor", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000360", "Esna", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000361", "New Tiba", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000362", "Al ziynia", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000363", "Al Bayadieh", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000364", "Al Qarna", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000365", "Armant", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000366", "Al Tud", seededAt),
                    }
                },

                // 25. Qena
                new()
                {
                    Id = new Guid("a1000000-0000-0000-0000-000000000025"),
                    Name = "Qena",
                    CreatedAt = seededAt,
                    CreatedBy = SeedActor,
                    Cities = new List<City>
                    {
                        NewCity("c1000000-0000-0000-0000-000000000367", "Qena", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000368", "New Qena", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000369", "Abu Tesht", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000370", "Nag Hammadi", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000371", "Deshna", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000372", "Alwaqf", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000373", "Qaft", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000374", "Naqada", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000375", "Farshout", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000376", "Quos", seededAt),
                    }
                },

                // 26. North Sinai
                new()
                {
                    Id = new Guid("a1000000-0000-0000-0000-000000000026"),
                    Name = "North Sinai",
                    CreatedAt = seededAt,
                    CreatedBy = SeedActor,
                    Cities = new List<City>
                    {
                        NewCity("c1000000-0000-0000-0000-000000000377", "Arish", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000378", "Sheikh Zowaid", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000379", "Nakhl", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000380", "Rafah", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000381", "Bir al-Abed", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000382", "Al Hasana", seededAt),
                    }
                },

                // 27. Sohag
                new()
                {
                    Id = new Guid("a1000000-0000-0000-0000-000000000027"),
                    Name = "Sohag",
                    CreatedAt = seededAt,
                    CreatedBy = SeedActor,
                    Cities = new List<City>
                    {
                        NewCity("c1000000-0000-0000-0000-000000000383", "Sohag", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000384", "Sohag El Gedida", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000385", "Akhmeem", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000386", "Akhmim El Gedida", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000387", "Albalina", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000388", "El Maragha", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000389", "almunsha'a", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000390", "Dar AISalaam", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000391", "Gerga", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000392", "Jahina Al Gharbia", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000393", "Saqilatuh", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000394", "Tama", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000395", "Tahta", seededAt),
                        NewCity("c1000000-0000-0000-0000-000000000396", "Alkawthar", seededAt),
                    }
                },
            };

            context.Areas.AddRange(areas);
            await context.SaveChangesAsync(cancellationToken);
        }

        // AreaId is left unset — EF assigns it from the owning Area's Cities collection.
        private static City NewCity(string id, string name, DateTime seededAt) => new()
        {
            Id = new Guid(id),
            Name = name,
            CreatedAt = seededAt,
            CreatedBy = SeedActor
        };
    }
}
