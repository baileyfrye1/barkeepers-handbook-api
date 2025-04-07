using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Supabase.Postgrest;
using Supabase.Postgrest.Interfaces;

namespace api.Helpers
{
    public class PostgrestQueryFilter : IPostgrestQueryFilter
    {
        public string? Property { get; set; }
        public Constants.Operator Op { get; set; }
        public object? Criteria { get; set; }
        public PostgrestQueryFilter(string? property, Constants.Operator op, object? criteria)
        {
            Property = property;
            Op = op;
            Criteria = criteria;
        }
    }
}