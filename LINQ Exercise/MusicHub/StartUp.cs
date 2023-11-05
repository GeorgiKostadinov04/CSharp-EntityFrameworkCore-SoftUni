namespace MusicHub
{
    using System;
    using System.Text;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;

    public class StartUp
    {
        public static void Main()
        {
            MusicHubDbContext context =
                new MusicHubDbContext();

            DbInitializer.ResetDatabase(context);

            Console.WriteLine(ExportSongsAboveDuration(context, 4));
        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            var albums = context.Producers.Include(a => a.Albums).ThenInclude(a => a.Songs).ThenInclude(a => a.Writer)
                .First(a => a.Id == producerId)
                .Albums.Select(x => new
                {
                    AlbumName = x.Name,
                    ReleaseDate = x.ReleaseDate,
                    ProducerName = x.Producer.Name,
                    AlbumSongs = x.Songs.Select(x => new
                    {
                        SongName = x.Name,
                        SongPrice = x.Price,
                        SongWriterName = x.Writer.Name
                    }).OrderByDescending(x => x.SongName).ThenBy(x => x.SongWriterName),
                    TotalAlbumPrice = x.Price
                }).OrderByDescending(x => x.TotalAlbumPrice).AsEnumerable();

            var result = new StringBuilder();

            foreach (var album in albums)
            {
                result.AppendLine($"-AlbumName: {album.AlbumName}")
                    .AppendLine($"-ReleaseDate: {album.ReleaseDate.ToString("MM/dd/yyyy")}")
                    .AppendLine($"-ProducerName: {album.ProducerName}")
                    .AppendLine($"-Songs:");


                int counter = 1;

                foreach (var song in album.AlbumSongs)
                {
                    result.AppendLine($"---#{counter++}")
                        .AppendLine($"---SongName: {song.SongName}")
                        .AppendLine($"---Price: {song.SongPrice:f2}")
                        .AppendLine($"---Writer: {song.SongWriterName}");
                }
                result.AppendLine($"-AlbumPrice: {album.TotalAlbumPrice:f2}");
            }
            return result.ToString().TrimEnd();

        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            var songs = context.Songs.
                Include(s => s.SongPerformers)
                    .ThenInclude(sp => sp.Performer)
                .Include(s => s.Writer)
                .Include(s => s.Album)
                    .ThenInclude(s => s.Producer)
                    .AsEnumerable()
                .Where(s => s.Duration.TotalSeconds > duration)
                .Select(s => new
                {
                    s.Name,
                    Performers = s.SongPerformers
                            .Select(sp => sp.Performer.FirstName + " " + sp.Performer.LastName)
                            .OrderBy(p=>p)
                            .ToList(),
                    WriterName = s.Writer.Name,
                    AlbumProducer = s.Album.Producer.Name,
                    Duration = s.Duration.ToString("c")
                }).OrderBy(s => s.Name)
                    .ThenBy(s => s.WriterName)
                    .ToList();

            int counter = 1;
            var result = new StringBuilder();
            foreach (var song in songs)
            {
                result.AppendLine($"-Song #{counter++}");
                result.AppendLine($"---SongName: {song.Name}");
                result.AppendLine($"---Writer: {song.WriterName}");

                foreach(var p in song.Performers)
                {
                    result.AppendLine($"---Performer: {p}");
                }

                result.AppendLine($"---AlbumProducer: {song.AlbumProducer}");
                result.AppendLine($"---Duration: {song.Duration}");
            }
            return result.ToString().TrimEnd();
        }
    }
}