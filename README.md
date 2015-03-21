Turbocharged.Beanstalk
======================

[![Build status](https://ci.appveyor.com/api/projects/status/9ydx1vwh8hjxhv4w?svg=true)](https://ci.appveyor.com/project/jennings/turbocharged-beanstalk)

A .NET library for using [Beanstalk][beanstalk].

There are other libraries, but they seem to have been abandoned:

* [libBeanstalk.NET][libbeanstalk]
* [Beanstalk-Sharp][beanstalk-sharp]


Usage
-----

    // Jobs are byte arrays
    byte[] job = new byte[] { 102, 105, 101, 116, 123, 124, 101, 114, 113 };

    // A producer exposes methods used for inserting jobs
    // Most producer methods are affected by UseAsync(tube)
    var producer = await BeanstalkConnection.ConnectProducerAsync(hostname, port);
    await producer.UseAsync("mytube");
    await producer.PutAsync(job, 5, TimeSpan.Zero, TimeSpan.FromSeconds(30));

    // A consumer exposes methods for reserving and deleting jobs
    // Most consumer methods are affected by WatchAsync(tube)
    var consumer = await BeanstalkConnection.ConnectConsumerAsync(hostname, port);
    await consumer.WatchAsync("mytube");
    var job = await consumer.ReserveAsync();

    // ...work work work...

    if (success)
        await consumer.DeleteAsync(job.Id);
    else
        await consumer.BuryAsync(job.Id, priority: 5);

    producer.Dispose();
    consumer.Dispose();


Goals
-----

* Simple API that encourages ease of use
* Lots of `async` happiness


License
-------

The MIT License. See `LICENSE.md`.


[beanstalk]: http://kr.github.io/beanstalkd/
[libbeanstalk]: https://github.com/sdether/libBeanstalk.NET
[beanstalk-sharp]: https://github.com/jtdowney/beanstalk-sharp
