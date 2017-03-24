using SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SignalRWalkthrough2.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        //
        // GET: /Chat/

        public ActionResult Index()
        {
            return View();
        }

    }

    public class Chat : Hub, IDisconnect
    {
        // join accepts multiple input params
        // i'm not actually using "name" here, just wanted to show in debugging that i 
        // could add multiple params if i wanted
        public Task Join(string group = "fee", string name = "Marvin")
        {
            // the .name prop on Caller was added on the client side
            Clients[group].addMessage(Caller.name + " has joined the '" + group + "' group.");
            return Groups.Add(Context.ConnectionId, group);
        }

        // send can also accept multiple params
        public void Send(string message, string group = "foo")
        {
            var msg = DateTime.Now.ToShortTimeString() + " (" + Context.ConnectionId + "): " + message;

            if (group == "Everyone")
            {
                Clients.addMessageFromName(Caller.name, message);
            }
            else
                Clients[group].addMessage(msg);
        }

        public void EchoPerson(Person person)
        {
            Clients.addMessage(person.Name
                + ", age: "
                + person.Age.ToString()
                + " was extracted.");
        }

        public void PingBackToCaller()
        {
            Caller.addMessage("You called.");
        }

        // fires on refresh or leaving the page
        public Task Disconnect()
        {
            return Clients["foo"].leave(Context.ConnectionId);
        }

        // from SignalR docs, doesn't work though
        //public string ExtractName(Person person)
        //{
        //    return person.Name; // just returns an empty object
        //}
    }

    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
