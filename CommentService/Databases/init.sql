--
-- PostgreSQL database dump
--

\restrict z9eeyL7aNvsdeNy4kMmpOPzW5yUSv73AbPbykZmyZrwVrJInyAK0Y6s4bmyzMdN

-- Dumped from database version 16.13 (Ubuntu 16.13-0ubuntu0.24.04.1)
-- Dumped by pg_dump version 16.13 (Ubuntu 16.13-0ubuntu0.24.04.1)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: Comments; Type: TABLE; Schema: public; Owner: pauldor
--

CREATE TABLE public."Comments" (
    "Id" uuid NOT NULL,
    "PersonId" text NOT NULL,
    "MusicId" uuid,
    "ParentId" uuid,
    "CreatedAt" timestamp with time zone NOT NULL
);


ALTER TABLE public."Comments" OWNER TO pauldor;

--
-- Name: Persons; Type: TABLE; Schema: public; Owner: pauldor
--

CREATE TABLE public."Persons" (
    "Id" text NOT NULL,
    "Name" text NOT NULL
);


ALTER TABLE public."Persons" OWNER TO pauldor;

--
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: public; Owner: pauldor
--

CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


ALTER TABLE public."__EFMigrationsHistory" OWNER TO pauldor;

--
-- Data for Name: Comments; Type: TABLE DATA; Schema: public; Owner: pauldor
--

COPY public."Comments" ("Id", "PersonId", "MusicId", "ParentId", "CreatedAt") FROM stdin;
\.


--
-- Data for Name: Persons; Type: TABLE DATA; Schema: public; Owner: pauldor
--

COPY public."Persons" ("Id", "Name") FROM stdin;
\.


--
-- Data for Name: __EFMigrationsHistory; Type: TABLE DATA; Schema: public; Owner: pauldor
--

COPY public."__EFMigrationsHistory" ("MigrationId", "ProductVersion") FROM stdin;
20260409162352_InitialCreate	10.0.5
20260410102651_response	10.0.5
20260410122502_personRelations	10.0.5
\.


--
-- Name: Comments PK_Comments; Type: CONSTRAINT; Schema: public; Owner: pauldor
--

ALTER TABLE ONLY public."Comments"
    ADD CONSTRAINT "PK_Comments" PRIMARY KEY ("Id");


--
-- Name: Persons PK_Persons; Type: CONSTRAINT; Schema: public; Owner: pauldor
--

ALTER TABLE ONLY public."Persons"
    ADD CONSTRAINT "PK_Persons" PRIMARY KEY ("Id");


--
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: public; Owner: pauldor
--

ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


--
-- Name: IX_Comments_ParentId; Type: INDEX; Schema: public; Owner: pauldor
--

CREATE INDEX "IX_Comments_ParentId" ON public."Comments" USING btree ("ParentId");


--
-- Name: IX_Comments_PersonId; Type: INDEX; Schema: public; Owner: pauldor
--

CREATE INDEX "IX_Comments_PersonId" ON public."Comments" USING btree ("PersonId");


--
-- Name: Comments FK_Comments_Comments_ParentId; Type: FK CONSTRAINT; Schema: public; Owner: pauldor
--

ALTER TABLE ONLY public."Comments"
    ADD CONSTRAINT "FK_Comments_Comments_ParentId" FOREIGN KEY ("ParentId") REFERENCES public."Comments"("Id") ON DELETE CASCADE;


--
-- Name: Comments FK_Comments_Persons_PersonId; Type: FK CONSTRAINT; Schema: public; Owner: pauldor
--

ALTER TABLE ONLY public."Comments"
    ADD CONSTRAINT "FK_Comments_Persons_PersonId" FOREIGN KEY ("PersonId") REFERENCES public."Persons"("Id") ON DELETE CASCADE;


--
-- PostgreSQL database dump complete
--

\unrestrict z9eeyL7aNvsdeNy4kMmpOPzW5yUSv73AbPbykZmyZrwVrJInyAK0Y6s4bmyzMdN

