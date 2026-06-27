namespace ListingService.Infrastructure.Persistence.Seeding;

internal static class CategorySubcategorySeedData
{
    internal sealed record SubCategorySeed(string Name, string Slug, string IconKey, int Sort, string? SearchTerm = null);

    internal static IReadOnlyDictionary<string, IReadOnlyList<SubCategorySeed>> ByParent { get; } =
        new Dictionary<string, IReadOnlyList<SubCategorySeed>>
        {
            ["mobiles"] =
            [
                new("All Mobiles", "all-mobiles", "all", 0),
                new("Mobile Phones", "mobile-phones", "mobile-phones", 1, "phone"),
                new("Mobile Phone Accessories", "mobile-accessories", "mobile-accessories", 2, "accessories"),
                new("Mobile Spare Parts", "mobile-spare-parts", "mobile-spare-parts", 3, "spare parts"),
                new("Smart Watches & Fitness Bands", "smart-watches", "smart-watches", 4, "watch"),
            ],
            ["electronics"] =
            [
                new("All Electronics", "all-electronics", "all", 0),
                new("Computers & Tablets", "computers-tablets", "computers", 1, "computer tablet"),
                new("Computer Accessories", "computer-accessories", "headphones", 2, "accessories"),
                new("TVs", "tvs", "tv", 3, "tv"),
                new("TV & Video Accessories", "tv-accessories", "tv-4k", 4, "video"),
                new("Cameras & Camcorders", "cameras", "camera", 5, "camera"),
                new("Audio & MP3", "audio", "microphone", 6, "audio"),
                new("Electronic Home Appliances", "appliances", "appliance", 7, "appliance"),
                new("Air Conditions & Electrical fittings", "air-condition", "fan", 8, "ac air condition"),
                new("Video Games & Consoles", "games", "gamepad", 9, "game console"),
                new("Other Electronics", "other-electronics", "speaker", 10, "electronics"),
            ],
            ["vehicles"] =
            [
                new("All Vehicles", "all-vehicles", "all", 0),
                new("Cars", "cars", "cars", 1, "car"),
                new("Motorbikes", "motorbikes", "motorbikes", 2, "bike motorbike"),
                new("Three Wheelers", "three-wheelers", "three-wheelers", 3, "tuk three wheeler"),
                new("Bicycles", "bicycles", "bicycles", 4, "bicycle"),
                new("Vans", "vans", "vans", 5, "van"),
                new("Buses", "buses", "buses", 6, "bus"),
                new("Lorries & Trucks", "trucks", "trucks", 7, "truck lorry"),
                new("Heavy Duty", "heavy-duty", "heavy-duty", 8, "heavy"),
                new("Tractors", "tractors", "tractors", 9, "tractor"),
                new("Auto Services", "auto-services", "auto-services", 10, "auto service"),
                new("Rentals", "vehicle-rentals", "rentals", 11, "rental"),
                new("Auto Parts & Accessories", "auto-parts", "auto-parts", 12, "parts"),
                new("Maintenance and Repair", "auto-repair", "auto-repair", 13, "repair"),
                new("Boats & Water Transport", "boats", "boats", 14, "boat"),
            ],
            ["property"] =
            [
                new("All Property", "all-property", "all", 0),
                new("Land For Sale", "land-sale", "land", 1, "land sale"),
                new("Land Rentals", "land-rent", "land", 2, "land rent"),
                new("Houses For Sale", "houses-sale", "house", 3, "house sale"),
                new("House Rentals", "house-rent", "house", 4, "house rent"),
                new("Apartments For Sale", "apartments-sale", "apartment", 5, "apartment sale"),
                new("Apartment Rentals", "apartments-rent", "apartment", 6, "apartment rent"),
                new("Commercial Properties For Sale", "commercial-sale", "commercial", 7, "commercial sale"),
                new("Commercial Property Rentals", "commercial-rent", "commercial", 8, "commercial rent"),
                new("Room & Annex Rentals", "room-rent", "room", 9, "room annex"),
                new("Holiday & Short-Term Rental", "holiday-rent", "holiday", 10, "holiday short term"),
            ],
            ["home-garden"] =
            [
                new("All Home & Garden", "all-home-garden", "all", 0),
                new("Furniture", "furniture", "furniture", 1, "furniture"),
                new("Bathroom & Sanitary ware", "bathroom", "bathroom", 2, "bathroom"),
                new("Garden", "garden", "garden", 3, "garden"),
                new("Home Decor", "home-decor", "sofa", 4, "decor"),
                new("Kitchen items", "kitchen", "kitchen", 5, "kitchen"),
                new("Other Home Items", "other-home", "chair", 6, "home"),
            ],
            ["animals"] =
            [
                new("All Animals", "all-animals", "all", 0),
                new("Pets", "pets", "pets", 1, "pet dog cat"),
                new("Pet Food", "pet-food", "pet-food", 2, "pet food"),
                new("Veterinary Services", "vet", "vet", 3, "veterinary"),
                new("Farm Animals", "farm-animals", "farm", 4, "farm animal"),
                new("Animal Accessories", "animal-accessories", "rabbit", 5, "animal accessories"),
                new("Other Animals", "other-animals", "paw", 6, "animal"),
            ],
            ["services"] =
            [
                new("All Services", "all-services", "all", 0),
                new("Trade Services", "trade-services", "hard-hat", 1, "trade"),
                new("Domestic Services", "domestic-services", "spray", 2, "domestic cleaning"),
                new("Events & Entertainment", "events", "stage", 3, "event entertainment"),
                new("Health & Wellbeing", "health", "health", 4, "health"),
                new("Travel & Tourism", "travel", "airplane", 5, "travel tourism"),
                new("Other Services", "other-services", "gear", 6, "service"),
            ],
            ["business-industry"] =
            [
                new("All Business & Industry", "all-business", "all", 0),
                new("Office Equipment, Supplies & Stationery", "office", "tools", 1, "office"),
                new("Solar & Generators", "solar", "solar", 2, "solar generator"),
                new("Industry Tools & Machinery", "machinery", "tools", 3, "machinery"),
                new("Raw Materials & Wholesale Lots", "raw-materials", "blocks", 4, "wholesale"),
                new("Licences & Titles", "licences", "certificate", 5, "licence"),
                new("Healthcare, Medical Equipment & Supplies", "medical", "stethoscope", 6, "medical"),
                new("Building Material & Tools", "building-material", "wheelbarrow", 7, "building material"),
                new("Other Business Services", "other-business", "handshake", 8, "business"),
            ],
            ["jobs"] =
            [
                new("All Jobs", "all-jobs", "all", 0),
                new("Data Entry Operator", "data-entry", "briefcase", 1, "data entry"),
                new("Driver", "driver", "car", 2, "driver"),
                new("Clerk", "clerk", "briefcase", 3, "clerk"),
                new("Sales Executive", "sales", "briefcase", 4, "sales"),
                new("IT & Software", "it-jobs", "monitor", 5, "software it"),
                new("Accounting & Finance", "finance-jobs", "briefcase", 6, "accounting finance"),
            ],
            ["hobby-sport-kids"] =
            [
                new("All Hobby, Sport & Kids", "all-hobby", "all", 0),
                new("Musical Instruments", "music-instruments", "guitar", 1, "instrument"),
                new("Sports & Fitness", "sports", "table-tennis", 2, "sport fitness"),
                new("Sports Supplements", "supplements", "supplement", 3, "supplement"),
                new("Travel, Events & Tickets", "tickets", "tickets", 4, "ticket"),
                new("Art & Collectibles", "art", "palette", 5, "art collectible"),
                new("Music, Books & Movies", "books", "cd", 6, "book movie"),
                new("Children's Items", "children", "teddy", 7, "children kids"),
                new("Other Hobby, Sport & Kids Items", "other-hobby", "puzzle", 8, "hobby"),
            ],
            ["fashion-beauty"] =
            [
                new("All Fashion & Beauty", "all-fashion", "all", 0),
                new("Bags & Luggage", "bags", "handbag", 1, "bag luggage"),
                new("Clothing", "clothing", "hanger", 2, "clothing"),
                new("Shoes & Footwear", "shoes", "shoe", 3, "shoe footwear"),
                new("Jewellery", "jewellery", "diamond", 4, "jewellery"),
                new("Sunglasses & Opticians", "sunglasses", "glasses", 5, "sunglasses"),
                new("Watches", "watches", "watch", 6, "watch"),
                new("Beauty Products", "beauty", "cosmetic", 7, "beauty cosmetic"),
                new("Other Personal Items", "other-fashion", "person", 8, "fashion"),
            ],
            ["essentials"] =
            [
                new("All Essentials", "all-essentials", "all", 0),
                new("Grocery", "grocery", "basket", 1, "grocery"),
                new("Fruits & Vegetables", "fruits", "fruits", 2, "fruit vegetable"),
                new("Meat & Seafood", "meat", "fish", 3, "meat seafood"),
                new("Baby Products", "baby", "baby", 4, "baby"),
                new("Healthcare", "healthcare", "health-cross", 5, "healthcare"),
                new("Household", "household", "house-small", 6, "household"),
                new("Gas", "gas", "gas", 7, "gas"),
                new("Other Essentials", "other-essentials", "cart", 8, "essentials"),
            ],
            ["education"] =
            [
                new("All Education", "all-education", "all", 0),
                new("Higher Education", "higher-education", "podium", 1, "university"),
                new("Textbooks", "textbooks", "book-closed", 2, "textbook"),
                new("Tuition", "tuition", "tuition", 3, "tuition"),
                new("Vocational Institutes", "vocational", "vocational", 4, "vocational"),
                new("Other Education", "other-education", "book-open", 5, "education"),
            ],
            ["agriculture"] =
            [
                new("All Agriculture", "all-agriculture", "all", 0),
                new("Crops, Seeds & Plants", "crops", "sprout", 1, "crop seed plant"),
                new("Farming Tools & Machinery", "farming-tools", "rake", 2, "farming tool"),
                new("Other Agriculture", "other-agriculture", "fence", 3, "agriculture"),
            ],
            ["work-overseas"] =
            [
                new("All Work Overseas", "all-overseas", "all", 0),
                new("Overseas Jobs", "overseas-jobs", "suitcase-globe", 1, "overseas job"),
                new("Study & Work Abroad", "study-abroad", "study-abroad", 2, "study abroad"),
            ],
        };

    internal static IReadOnlyDictionary<string, string> ParentIconKeys { get; } =
        new Dictionary<string, string>
        {
            ["vehicles"] = "vehicles",
            ["property"] = "property",
            ["electronics"] = "electronics",
            ["mobiles"] = "mobiles",
            ["services"] = "services",
            ["home-garden"] = "home-garden",
            ["business-industry"] = "business-industry",
            ["jobs"] = "jobs",
            ["animals"] = "animals",
            ["hobby-sport-kids"] = "hobby-sport-kids",
            ["fashion-beauty"] = "fashion-beauty",
            ["education"] = "education",
            ["essentials"] = "essentials",
            ["agriculture"] = "agriculture",
            ["work-overseas"] = "work-overseas",
            ["other"] = "other",
        };
}
