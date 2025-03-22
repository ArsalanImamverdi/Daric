namespace Daric.Database.Abstraction.Models.Pagination
{
    public class PagesResultModel
    {
        public static PagesResultModel<TData> Create<TData>(IList<TData> data)
        {
            return new PagesResultModel<TData>()
            {
                Count = data.Count,
                Data = data.ToList()
            };
        }
    }

    public class PagesResultModel<T> : PagesResultModel
    {
        public List<T> Data { get; set; }
        public long Count { get; set; }

        public PagesResultModel()
        {
            Data = new List<T>();
        }

        public static implicit operator PagesResultModel<T>((long Count, List<T> Data) result)
        {
            return new PagesResultModel<T>()
            {
                Count = result.Count,
                Data = result.Data
            };
        }

        public static implicit operator PagesResultModel<T>(List<T> items)
        {
            return new PagesResultModel<T>()
            {
                Count = items.Count,
                Data = items
            };
        }
    }

}
