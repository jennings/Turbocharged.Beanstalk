Turbocharged.Beanstalk
======================

[![Build status](https://ci.appveyor.com/api/projects/status/9ydx1vwh8hjxhv4w?svg=true)](https://ci.appveyor.com/project/jennings/turbocharged-beanstalk)

A .NET library for using [Beanstalk][beanstalk].

There are other libraries, but they seem to have been abandoned:

* [libBeanstalk.NET][libbeanstalk]
* [Beanstalk-Sharp][beanstalk-sharp]


Usage
-----

    var connection = new BeanstalkConnection("localhost", 11300);

    var producer = connection.GetProducer();
    await producer.PutAsync(new [] {}, priority: 5, delay: 0, timeToRun: 60);

    var consumer = connection.GetConsumer();
    var job = await consumer.ReserveAsync();

    // ...work work work...

    await consumer.DeleteAsync(job.Id);


Goals
-----

* Simple API
* Lots of `async` happiness


License
-------

The MIT License. See `LICENSE.md`.


[beanstalk]: http://kr.github.io/beanstalkd/
[libbeanstalk]: https://github.com/sdether/libBeanstalk.NET
[beanstalk-sharp]: https://github.com/jtdowney/beanstalk-sharp
