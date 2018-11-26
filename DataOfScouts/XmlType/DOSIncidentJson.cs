namespace DOSIncidentJson
{
    public class IncidentJson
    {
        public int id { get; set; }
        public string type { get; set; }
        public int source { get; set; }
        public int ut { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public Incident incident { get; set; }
        public Event @event { get; set; }
    }

    public class Incident
    {
        public string id { get; set; }
        public string action { get; set; }
        public int incident_id { get; set; }
        public string incident_name { get; set; }
        public object participant_id { get; set; }
        public object participant_name { get; set; }
        public object subparticipant_id { get; set; }
        public object subparticipant_name { get; set; }
        public object info { get; set; }
        public string important { get; set; }
        public string important_for_trader { get; set; }
        public object add_data { get; set; }
        public string show_popup { get; set; }
        public string show_scores { get; set; }
        public string show_action { get; set; }
        public string show_time { get; set; }
        public string show_on_timeline { get; set; }
        public string event_time { get; set; }
        public int event_status_id { get; set; }
        public string event_status_name { get; set; }
    }

    public class Event
    {
        public int id { get; set; }
        public string action { get; set; }
        public string start_date { get; set; }
        public string ft_only { get; set; }
        public string coverage_type { get; set; }
        public int status_id { get; set; }
        public int sport_id { get; set; }
        public object day { get; set; }
        public string neutral_venue { get; set; }
        public string item_status { get; set; }
        public string clock_time { get; set; }
        public string clock_status { get; set; }
        public int area_id { get; set; }
        public int competition_id { get; set; }
        public int season_id { get; set; }
        public int stage_id { get; set; }
        public object tour_id { get; set; }
        public string gender { get; set; }
        public string bet_status { get; set; }
        public string relation_status { get; set; }
        public string status_type { get; set; }
        public string name { get; set; }
        public object round_id { get; set; }
        public object round_name { get; set; }
        public Detail[] details { get; set; }
        public Participant[] participants { get; set; }
    }

    public class Detail
    {
        public int id { get; set; }
        public object value { get; set; }
    }

    public class Participant
    {
        public int id { get; set; }
        public int counter { get; set; }
        public string name { get; set; }
        public string short_name { get; set; }
        public string acronym { get; set; }
        public int area_id { get; set; }
        public string area_name { get; set; }
        public string area_code { get; set; }
        public int ut { get; set; }
        public Stat[] stats { get; set; }
        public Result[] results { get; set; }
    }

    public class Stat
    {
        public int id { get; set; }
        public string value { get; set; }
    }

    public class Result
    {
        public int id { get; set; }
        public int? value { get; set; }
    }

}