using System;
using System.Linq;

namespace JudgeDesignProblem
{
    class Program
    {
        static void Main(string[] args)
        {
            //Create the solver
            ISolver solver = new TheaterSeatingSolver();

            //Read data
            solver.Read();
            
            //Ensure that data is equal to the input
            solver.PrintData();

            //Solve the problem
            solver.Solve();

            //Ensure the data printed as expected to the solution
            solver.PrintData();
        }
    }

    /// <summary>
    /// Helps to hide the parts of the problem which are domain specific
    /// </summary>
    public interface ISolver
    {
        /// <summary>
        /// Reads data from the console to prepare the problem
        /// </summary>
        void Read();
        /// <summary>
        /// Prints the input data which was read in <see cref="Read"/>
        /// </summary>
        void PrintData();
        /// <summary>
        /// Solves the problem domain data. May write data to the <see cref="Console"/> or perform whatever logic is required to solve the problem.
        /// </summary>
        void Solve();
    }

    /// <summary>
    /// A class which implements <see cref="ISolver"/> and contains logic to solve Problem 2: Theatre Seating
    /// </summary>
    class TheaterSeatingSolver : ISolver
    {
        /// <summary>
        /// A class which represents the seating row
        /// </summary>
        class Row
        {
            public readonly System.Collections.Generic.List<Section> Sections = new System.Collections.Generic.List<Section>();
            public int Remaining { get { return Sections.Sum(s => s.Remaining); } }

            public void AddSection(int seating)
            {
                Sections.Add(new Section(seating));
            }
        }

        /// <summary>
        /// A class which represents the section seating and features.
        /// </summary>
        class Section
        {
            public readonly int Capacity;

            public int Remaining;

            public bool HandicappedAccess;

            public System.Collections.Generic.List<Reservation> Reservations = new System.Collections.Generic.List<Reservation>();

            public Section(int capacity)
            {
                Capacity = capacity;

                Remaining = Capacity;
            }

        }

        /// <summary>
        /// Represents the names and seats of those in the section.
        /// </summary>
        public class Reservation 
        { 
            public string Name; 
            public int Seats; 
        }


        /// <summary>
        /// Data stored for solving during <see cref="Read"/>
        /// </summary>
        readonly System.Collections.Generic.List<Row> m_Rows = new System.Collections.Generic.List<Row>();

        /// <summary>
        /// Indicates if <see cref="Solve"/> will be able to be called
        /// </summary>
        public bool CanSolve { get; set; }

        /// <summary>
        /// Indicates if <see cref="Solve"/> has completed work and the solution is ready
        /// </summary>
        public bool IsSolved { get; set; }

        /// <summary>
        /// Reads the data from the <see cref="Console"/> until an empty line is reached.
        /// </summary>
        public void Read()
        {
            CanSolve = IsSolved = false;
            m_Rows.Clear();
            string input;
        ReadInput:
            input = System.Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                CanSolve = true;
                return;
            }
            string[] values = input.Split(" ");
            int seating;
            Row row = new Row();
            foreach (string value in values)
            {
                if (int.TryParse(value, out seating))
                {
                    row.AddSection(seating);
                }
            }
            m_Rows.Add(row);
            goto ReadInput;
        }

        /// <summary>
        /// A private method to print the rows and sections
        /// </summary>
        void PrintRows()
        {
            for (int i = 0, e = m_Rows.Count; i < e; ++i)
            {
                Row r = m_Rows[i];
                for (int s = 0, z = r.Sections.Count; s < z; ++s)
                {
                    Section section = r.Sections[s];
                    System.Console.Write(section.Remaining);
                    System.Console.Write(" ");
                }
                System.Console.WriteLine();
            }
        }

        /// <summary>
        /// Prints the input data to the <see cref="Console"/>
        /// </summary>
        public void PrintData()
        {
            PrintRows();
        }

        public string TryMakeReservation(Reservation reservation)
        {
            //If the reservation cannot fit in the theatre then indicate that
            if (reservation.Seats > m_Rows.Sum(r => r.Sections.Sum(s => s.Capacity))) return "\"Sorry, we can't handle your party \"";

            //Loop from the front to get best available seating.
            for (int i = m_Rows.Count - 1; i >= 0; --i)
            {
                Row r = m_Rows[i];
                if (r.Remaining < reservation.Seats) continue;
                for (int s = r.Sections.Count - 1; s >= 0; --s)
                {
                    Section section = r.Sections[s];
                    if (reservation.Seats > section.Capacity) continue;
                    if (section.Remaining < reservation.Seats) continue;
                    section.Reservations.Add(reservation);
                    section.Remaining -= reservation.Seats;
                    return string.Join(" ", reservation.Name, "Row", i, "Section", s);
                }
            }

            //The party can be handled but needs to be split up
            return reservation.Name + " Call to split party";
        }

        /// <summary>
        /// Solves the problem by reading the problem data from and writing data to the <see cref="Console"/>
        /// </summary>
        public void Solve()
        {
            if (CanSolve is false) return;

            string input;
        ReadInput:
            input = System.Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) goto Exit;
            string[] values = input.Split(" ");
            int seating;
            Reservation reservation = new Reservation();
            if(values.Length > 1 && int.TryParse(values[1], out seating))
            {
                reservation.Name = values[0];
                reservation.Seats = seating;
                System.Console.WriteLine(TryMakeReservation(reservation));
            }
            goto ReadInput;
        Exit:
            IsSolved = true;
        }
    }


}
