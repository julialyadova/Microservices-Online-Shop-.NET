using System;

namespace Api.Config
{
    public class AppConfig
    {
        public string DefaultStatus { get; set; }
        public int DefaultWidth { get; set; }
        public int DefaultHeight { get; set; }
        public string DefaultColor { get; set; }

        public string MinioEndpoint = Environment.GetEnvironmentVariable("MINIO_ENDPOINT");
        public string MinioAccessKey = Environment.GetEnvironmentVariable("MINIO_ACCESS_KEY");
        public string MinioSecretKey = Environment.GetEnvironmentVariable("MINIO_SECRET_KEY");
        public string MinioCarpetsBucket = Environment.GetEnvironmentVariable("MINIO_CARPETS_BUCKET");

    }
}
