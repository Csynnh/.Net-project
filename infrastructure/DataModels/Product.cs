using System.ComponentModel;

namespace infrastructure.DataModels
{

    public class Product
    {
        public Guid id { get; set; }// Primary Key
        public string name { get; set; } = string.Empty;  // Tên của sản phẩm
        public string desc { get; set; } = string.Empty;  // Mô tả sản phẩm
        public decimal price { get; set; }  // Giá của sản phẩm
        public int inventory { get; set; }  // Số lượng sản phẩm còn trong kho
        public string image_url { get; set; } = string.Empty;  // Đường dẫn tới hình ảnh của sản phẩm
    }

    public class ExtField
    {
        public string weight { get; set; }
        public List<string> features { get; set; }
        public string material { get; set; }
        public string warranty { get; set; }
        public string model_number { get; set; }

        public static implicit operator string?(ExtField? v)
        {
            throw new NotImplementedException();
        }
    }

    public class ImageModel
    {
        public string main_image { get; set; }
        public List<string> additional_images { get; set; }
    }

    public class CollectionDBResponsse
    {
        public Guid id { get; set; }
        public string collection { get; set; } = string.Empty;
        public string image_url { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public decimal price { get; set; }
        public IEnumerable<string> available_colors { get; set; } = new List<string>();
    }

    public class CollectionResponse
    {
        public string collection { get; set; }
        public string name { get; set; }
        public string image_url { get; set; }
        public decimal price { get; set; }
        public List<string> available_colors { get; set; }
    }


    public class ProductDetailsDBResponse
    {
        public string name { get; set; }
        public string collection { get; set; }
        public object available_colors { get; set; }
        public decimal price { get; set; }
        public string description { get; set; }
        public int total_count { get; set; }
        public string images_per_color { get; set; }
        public string ext_fields { get; set; }
    }

    public class ProductDetailsResponse
    {
        public string collection { get; set; }
        public string name { get; set; }
        public decimal price { get; set; }
        public object available_colors { get; set; }
        public Dictionary<string, List<object>>? images_per_color { get; set; }
        public object ext_fields { get; set; }
    }

    public class ProductDBRequest
    {
        public ProductDBRequest(string name, string description, string collection, decimal price, object img_url, string color, object ext_fields, bool is_sold)
        {
            this.name = name;
            this.description = description;
            this.collection = collection;
            this.price = price;
            this.img_url = img_url;
            this.color = color;
            this.ext_fields = ext_fields;
            this.is_sold = is_sold;
        }
        public string name { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public string collection { get; set; } = string.Empty;
        public decimal price { get; set; } = 0;
        public object img_url { get; set; } = new { }; // Modified code
        public string color { get; set; } = string.Empty; // Modified code
        public object ext_fields { get; set; } = new { }; // Modified code
        public bool is_sold { get; set; } = false;
    }

    public class ProductRequest
    {
        public string name { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        [DefaultValue(Constants.DEFAULTCOLLECTION)]
        public string collection { get; set; } = Constants.DEFAULTCOLLECTION;
        public decimal price { get; set; } = 0;
        [DefaultValue(Constants.DEFAULTIMAGEURL)]
        public string main_img_url { get; set; } = string.Empty;
        [DefaultValue(new[] { Constants.DEFAULTIMAGEURL, Constants.DEFAULTIMAGEURL })]
        public List<string> additional_img_url { get; set; } = new List<string>();
        [DefaultValue(Constants.DEFAULTCOLOR)]
        public string color { get; set; } = Constants.DEFAULTCOLOR;
        [DefaultValue(Constants.DEFAULTEXTFIELDSVALUE)]
        public object ext_fields { get; set; } = new { };
        [DefaultValue(false)]
        public bool is_sold { get; set; } = false;
        [DefaultValue(1)]
        public int count { get; set; } = 1;
    }
}

