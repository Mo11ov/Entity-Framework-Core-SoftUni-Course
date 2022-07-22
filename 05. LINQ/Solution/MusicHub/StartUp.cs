namespace MusicHub
{
    using System;
    using System.Linq;
    using System.Text;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            MusicHubDbContext context = 
                new MusicHubDbContext();

            DbInitializer.ResetDatabase(context);


            //Task 02
            //Console.WriteLine(ExportAlbumsInfo(context, 9));

            //Task 03
            //Console.WriteLine(ExportSongsAboveDuration(context, 4));
        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            StringBuilder sb = new StringBuilder();

            var albums = context.Albums
                .Where(a => a.ProducerId == producerId)
                .Include(a => a.Producer)
                .Include(a => a.Songs)
                .ThenInclude(s => s.Writer)
                .ToList()
                .Select(a => new
                {
                    AlbumName = a.Name,
                    ReleaseDate = a.ReleaseDate.ToString("MM/dd/yyyy"),
                    ProducerName = a.Producer.Name,
                    AlbumSongs = a.Songs
                    .Select(s => new
                    {
                        songName = s.Name,
                        s.Price,
                        songWriter = s.Writer.Name
                    })
                    .ToList()
                    .OrderByDescending(s => s.songName)
                    .ThenBy(s => s.songWriter),
                    albumPrice = a.Price
                })
                .OrderByDescending(a => a.albumPrice);

            foreach (var album in albums)
            {
                sb.AppendLine($"-AlbumName: {album.AlbumName}");
                sb.AppendLine($"-ReleaseDate: {album.ReleaseDate}");
                sb.AppendLine($"-ProducerName: {album.ProducerName}");
                sb.AppendLine("-Songs:");

                int songsCount = 1;
                foreach (var song in album.AlbumSongs)
                {
                    sb.AppendLine($"---#{songsCount++}");
                    sb.AppendLine($"---SongName: {song.songName}");
                    sb.AppendLine($"---Price: {song.Price:f2}");
                    sb.AppendLine($"---Writer: {song.songWriter}");
                }

                sb.AppendLine($"-AlbumPrice: {album.albumPrice:f2}");
            }


            return sb.ToString().TrimEnd();
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            StringBuilder sb = new StringBuilder();

            var songs = context.Songs
                .Include(x => x.SongPerformers)
                .ThenInclude(x => x.Performer)
                .Include(x => x.Writer)
                .Include(x => x.Album)
                .ThenInclude(x => x.Producer)
                .ToList()
                .Where(s => s.Duration.TotalSeconds > duration)
                .Select(s => new
                {
                    s.Name,
                    Performer = s.SongPerformers
                    .Select(p => $"{ p.Performer.FirstName } { p.Performer.LastName }").FirstOrDefault(),
                    WriterName = s.Writer.Name,
                    AlbumProducer = s.Album.Producer.Name,
                    SongDuration = s.Duration.ToString("c")
                })
                .ToList()
                .OrderBy(s => s.Name)
                .ThenBy(s => s.WriterName)
                .ThenBy(s => s.Performer);

            int songsCounter = 1;

            foreach (var song in songs)
            {
                sb.AppendLine($"-Song #{songsCounter++}");
                sb.AppendLine($"---SongName: {song.Name}");
                sb.AppendLine($"---Writer: {song.WriterName}");
                sb.AppendLine($"---Performer: {song.Performer}");
                sb.AppendLine($"---AlbumProducer: {song.AlbumProducer}");
                sb.AppendLine($"---Duration: {song.SongDuration}");
            }

            return sb.ToString().TrimEnd();
        }
    }
}
