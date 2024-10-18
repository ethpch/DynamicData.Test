using DynamicData;
using System.Reactive.Linq;

namespace ConsoleDemo
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            SourceCache<TestRecord, int> cache = new(x => x.ID);
            cache.Connect()
                .Sample(TimeSpan.FromSeconds(1))

                // work well
                //.Transform(x => new TestRecord2(x))

                // error
                .TransformWithInlineUpdate(
                    transformFactory: x => new TestRecord2(x),
                    updateAction: (x2, x) =>
                    {
                        x2.ID = x.ID;
                        x2.Value = x.Value;
                    })

                .Subscribe(x =>
                {
                    foreach (var change in x)
                        Console.WriteLine($"Record ID = {change.Current.ID}, Value = {change.Current.Value}");
                });
            Observable.Timer(TimeSpan.Zero, TimeSpan.FromMilliseconds(100))
                .Subscribe(x =>
                {
                    cache.AddOrUpdate(new TestRecord() { ID = 1, Value = new Random().Next(1, 100) });
                });
            Console.ReadKey();
        }
    }
    class TestRecord
    {
        public int ID { get; set; }
        public int Value { get; set; }
    }
    class TestRecord2 : TestRecord
    {
        public TestRecord2(TestRecord record)
        {
            ID = record.ID;
            Value = record.Value;
        }
    }
}
