
namespace GeoCidadao.Cloud.Models.BucketRequests
{
    public class PutObjectRequest : BaseBucketRequest
    {
        public Stream FileContent { get; set; } = null!;
        public string CompanyToken { get; set; } = null!;
        public string StudyInstanceUID { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
        // public BucketTier StorageTier { get; set; }

        // public override string ToString()
        // {
        //     return CloudHelpers.ConvertPropertiesToObjectKey(companyToken: CompanyToken, studyInstanceUID: StudyInstanceUID, createdAt: CreatedAt);
        // }
    }
}