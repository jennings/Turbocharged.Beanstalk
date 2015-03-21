Turbocharged.Beanstalk
======================

[![Build status](https://ci.appveyor.com/api/projects/status/9ydx1vwh8hjxhv4w?svg=true)](https://ci.appveyor.com/project/jennings/turbocharged-beanstalk)

A .NET library for using [Beanstalk][beanstalk] filled with `async` happiness.

Don't like `async`? That's cool, no problem. You might like
[libBeanstalk.NET][libbeanstalk] instead.


Usage
-----

### Producing Jobs

    // Jobs are byte arrays
    byte[] job = new byte[] { 102, 105, 101, 116, 123, 124, 101, 114, 113 };

    // A producer exposes methods used for inserting jobs
    // Most producer methods are affected by UseAsync(tube)

    IProducer producer = await BeanstalkConnection.ConnectProducerAsync(hostname, port);
    await producer.UseAsync("mytube");
    await producer.PutAsync(job, 5, TimeSpan.Zero, TimeSpan.FromSeconds(30));

    producer.Dispose();

### Consuming jobs

    // A consumer exposes methods for reserving and deleting jobs
    // Most consumer methods are affected by WatchAsync(tube)

    IConsumer consumer = await BeanstalkConnection.ConnectConsumerAsync(hostname, port);
    await consumer.WatchAsync("mytube");
    var job = await consumer.ReserveAsync();

    // ...work work work...

    if (success)
        await consumer.DeleteAsync(job.Id);
    else
        await consumer.BuryAsync(job.Id, priority: 5);

    consumer.Dispose();

### Creating a worker task

    Func<IWorker, Job, Task> workerFunc = async (worker, job) =>
    {
        // ... work with the job...
        if (success)
            await worker.DeleteAsync(job.Id);
        else
            await worker.BuryAsync(job.Id, 1);
    };

    IDisposable worker = BeanstalkConnection.ConnectWorkerAsync(hostname, port, workerFunc);

    // When you're ready to stop the worker
    worker.Dispose();

A worker task is a dedicated BeanstalkConnection. You provide a delegate with
signature `Func<IWorker, Job, Task>` to process jobs and delete/bury them when
finished. Turbocharged.Beanstalk calls "reserve" for you and hands the reserved
job to your delegate.


Goals
-----

* Simple API that encourages ease of use
* Teach myself how to properly use the shiny asynchrony features in C# 5.0.


License
-------

The MIT License. See `LICENSE.md`.


[beanstalk]: http://kr.github.io/beanstalkd/
[libbeanstalk]: https://github.com/sdether/libBeanstalk.NET
