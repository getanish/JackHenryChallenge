namespace JackHenryChallenge.Models.Twitter
{
    public class GetSampleStreamRequest
    {
        public string Url { get => "/2/tweets/sample/stream"; }
        public readonly List<Field> AdditionalFields;

        public GetSampleStreamRequest(List<Field> additionalFields)
        {
            AdditionalFields = additionalFields;
        }

        public IDictionary<string, string> QueryStrings
        {
            get
            {
                var result = new Dictionary<string, string>();
                if (AdditionalFields.Any())
                {
                    result.Add("tweet.fields", string.Join(",", AdditionalFields.Select(x=>x.Name)));
                }
                return result;
            }


        }
    }
    
}
