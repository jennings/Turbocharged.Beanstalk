Turbocharged.Beanstalk
======================

[![NuGet](https://img.shields.io/nuget/v/Turbocharged.Beanstalk.svg)](http://www.nuget.org/packages/Turbocharged.Beanstalk/)

A [Beanstalk][beanstalk] .NET client library filled with `async` happiness.

Don't like `async`? That's cool, no problem. You might like
[libBeanstalk.NET][libbeanstalk] instead.


Usage
-----

Do the normal thing:

    PM> Install-Package Turbocharged.Beanstalk


Because of the way the Beanstalk protocol works, it's important that producers
and consumers use separate connections. So, when creating a
`BeanstalkConnection`, you need to choose whether it's a consumer or producer.



### Producing Jobs

Create a Producer if you need to insert jobs. Most producer methods are
affected by `UseAsync(tube)`.

```c#
IProducer producer = await BeanstalkConnection.ConnectProducerAsync("localhost:11300");
await producer.UseAsync("mytube");
```

Beanstalk jobs are just blobs, so jobs are represented as byte arrays.

```c#
byte[] job = new byte[] { 102, 105, 101, 116, 123, 124, 101, 114, 113 };
await producer.PutAsync(job, 5, TimeSpan.Zero, TimeSpan.FromSeconds(30));
```

Not feeling the love for byte arrays? You can also put custom objects and
they'll be serialized. The default is JSON.


```c#
await producer.PutAsync<MyObject>(obj, 5, TimeSpan.Zero, TimeSpan.FromSeconds(30));
```


Since Beanstalk maintains a TCP connection, you need to clean up your toys when
you're done:

```c#
producer.Dispose();
```



### Consuming jobs

If you need to consume jobs, create a Consumer instead. Most consumer methods
are affected by `WatchAsync(tube)` and `IgnoreAsync(tube)`.

```c#
IConsumer consumer = await BeanstalkConnection.ConnectConsumerAsync("localhost:11300");
await consumer.WatchAsync("mytube");
```

To ask Beanstalk for a job, reserve it:

```c#
Job job = await consumer.ReserveAsync();
// or: 
Job job = await consumer.ReserveAsync(timeout: TimeSpan.FromSeconds(10));

Console.WriteLine("Reserved job ID = {0}, Length = ", job.Id, job.Data.Length);
```


You can also deserialize if you know what type you're expecting.

```c#
Job<MyObject> job = await consumer.ReserveAsync<MyObject>();
```

When you're done with your job, ask your consumer to delete it or bury it.

```c#
if (success)
    await consumer.DeleteAsync(job.Id);
else
    await consumer.BuryAsync(job.Id, priority: 5);
```

Again, clean up after yourself when you don't need the connection anymore:

```c#
consumer.Dispose();
```



### Creating a worker task

A worker task is a `BeanstalkConnection` that processes jobs in a loop.

1. You provide a delegate with signature `Func<IWorker, Job, Task>`.
   Turbocharged.Beanstalk immediately connects and called "reserve" for you.

2. Your delegate gets called whenever a job is reserved.

3. Call DeleteAsync or BuryAsync when you're finished.


It looks like this:

```c#
private Task MyWorkerFunc(IWorker worker, Job job)
{
    bool success = ProcessJob(job.Data);
    if (success)
        await worker.DeleteAsync(job.Id);
    else
        await worker.BuryAsync(job.Id, 1);
}

IDisposable worker = BeanstalkConnection.ConnectWorkerAsync(hostname, port, MyWorkerFunc);

```


You can also use serialized messages:

```c#
private Task MyTypedWorkerFunc(IWorker worker, Job<MyObject> job)
{
    bool success = ProcessJob(job.Object);
    if (success)
        await worker.DeleteAsync(job.Id);
    else
        await worker.BuryAsync(job.Id, 1);
}

IDisposable worker = BeanstalkConnection.ConnectWorkerAsync<MyObject>(hostname, port, MyTypedWorkerFunc);
```


As usual, dispose the worker to make it stop.

```c#
worker.Dispose();
```


Goals
-----

* Simple API that encourages ease of use
* Teach myself how to properly use the shiny asynchrony features in C# 5.0.


License
-------

The MIT License. See `LICENSE.md`.


[beanstalk]: http://kr.github.io/beanstalkd/
[libbeanstalk]: https://github.com/sdether/libBeanstalk.NET
