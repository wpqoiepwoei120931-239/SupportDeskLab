using System;
using System.Collections.Generic;
using static SupportDeskLab.Utility;


namespace SupportDeskLab
{
   
     
    class Program
    {
        static int NextTicketId = 1;

        //Create Customer Dictionary
        static Dictionary<string, Customer> Customers = new Dictionary<string, Customer>();

        //create Ticket Queue
        static Queue<Ticket> Tickets = new Queue<Ticket>();

        //Create UndoEvent stack
        static Stack<UndoEvent> UndoStack = new Stack<UndoEvent>();

        static void Main()
        {
            initCustomer();

            while (true)
            {
                Console.WriteLine("\n=== Support Desk ===");
                Console.WriteLine("[1] Add customer");
                Console.WriteLine("[2] Find customer");
                Console.WriteLine("[3] Create ticket");
                Console.WriteLine("[4] Serve next ticket");
                Console.WriteLine("[5] List customers");
                Console.WriteLine("[6] List tickets");
                Console.WriteLine("[7] Undo last action");
                Console.WriteLine("[0] Exit");
                Console.Write("Choose: ");
                string choice = Console.ReadLine();

                //create switch cases and then call a reletive method 
                //for example for case 1 you need to have a method named addCustomer(); or case 2 add a method name findCustomer

                switch (choice)
                {
                    case "1": AddCustomer(); break;
                    case "2": FindCustomer(); break;
                    case "3": CreateTicket(); break;
                    case "4": ServeNext(); break;
                    case "5": ListCustomers(); break;
                    case "6": ListTickets(); break;
                    case "7": Undo(); break;
                    case "0": return;
                    default: Console.WriteLine("Invalid option."); break;
                }
            }
        }
        /*
         * Do not touch initCustomer method. this is like a seed to have default customers.
         */
        static void initCustomer()
        {
            //uncomments these 3 lines after you create the Customer Dictionary
            Customers["C001"] = new Customer("C001", "Ava Martin", "ava@example.com");
            Customers["C002"] = new Customer("C002", "Ben Parker", "ben@example.com");
            Customers["C003"] = new Customer("C003", "Chloe Diaz", "chloe@example.com");
        }

        static void AddCustomer()
        {
            //look at the Demo captuerd image and add your code here
            Console.Write("New CustomerId (e.g., C001): ");
            string id = Console.ReadLine();
            Console.Write("Name: ");
            string name = Console.ReadLine();
            Console.Write("Email: ");
            string email = Console.ReadLine();

            Customer c = new Customer(id, name, email);
            Customers[id] = c;
            UndoStack.Push(new UndoAddCustomer(c));

            Console.WriteLine($"Added: {id} | {name} | {email}");
        }

        static void FindCustomer()
        {
            //look at the Demo captuerd image and add your code here
            Console.Write("Enter CustomerId: ");
            string id = Console.ReadLine();

            if (Customers.ContainsKey(id))
            {
                Customer c = Customers[id];
                Console.WriteLine($"Found: {c.CustomerId} | {c.Name} | {c.Email}");
            }
            else
            {
                Console.WriteLine("Not found.");
            }
        }

        static void CreateTicket()
        {
            //look at the Demo captuerd image and add your code here
            Console.Write("CustomerId: ");
            string custId = Console.ReadLine();
            Console.Write("Subject: ");
            string subject = Console.ReadLine();

            Ticket t = new Ticket(NextTicketId, custId, subject);
            Tickets.Enqueue(t);
            UndoStack.Push(new UndoCreateTicket(t));

            Console.WriteLine($"Created ticket: #{NextTicketId} ({custId}) - {subject} @ {t.CreatedAt}");
            NextTicketId++;
        }

        static void ServeNext()
        {
            //look at the Demo captuerd image and add your code here
            if (Tickets.Count == 0)
            {
                Console.WriteLine("No tickets in queue.");
                return;
            }

            Ticket served = Tickets.Dequeue();
            UndoStack.Push(new UndoServeTicket(served));
            Console.WriteLine($"Served ticket: #{served.TicketId} ({served.CustomerId}) - {served.Subject} @ {served.CreatedAt}");
        }

        static void ListCustomers()
        {
            Console.WriteLine("-- Customers --");
            //look at the Demo captuerd image and add your code here
            foreach (var entry in Customers)
            {
                Customer c = entry.Value;
                Console.WriteLine($"{c.CustomerId} | {c.Name} | {c.Email}");
            }
        }

        static void ListTickets()
        {
           
            Console.WriteLine("-- Tickets (front to back) --");
            //look at the Demo captuerd image and add your code here
            foreach (Ticket t in Tickets)
            {
                Console.WriteLine($"#{t.TicketId} ({t.CustomerId}) - {t.Subject} @ {t.CreatedAt}");
            }
        }

        static void Undo()
        {
            //look at the Demo captuerd image and add your code here
            if (UndoStack.Count == 0)
            {
                Console.WriteLine("Nothing to undo.");
                return;
            }

            UndoEvent lastEvent = UndoStack.Pop();

            if (lastEvent is UndoAddCustomer undoAdd)
            {
                string custId = undoAdd.Customer.CustomerId;
                Customers.Remove(custId);
                Console.WriteLine($"Undo: Deleted customer {custId}");
            }
            else if (lastEvent is UndoCreateTicket undoCreate)
            {
                int ticketId = undoCreate.Ticket.TicketId;
                Queue<Ticket> temp = new Queue<Ticket>();

                foreach (Ticket t in Tickets)
                {
                    if (t.TicketId != ticketId)
                    {
                        temp.Enqueue(t);
                    }
                    else
                    {
                        Console.WriteLine($"Undo: Deleted ticket #{ticketId}");
                    }
                }
                Tickets = temp;
            }
            else if (lastEvent is UndoServeTicket undoServe)
            {
                Ticket t = undoServe.Ticket;
                Tickets.Enqueue(t);
                Console.WriteLine($"Undo: Re-added ticket #{t.TicketId}");
            }
        }
    }
}

