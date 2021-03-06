﻿using System;
using System.Linq;
using Pixie;
using Pixie.Terminal;
using Pixie.Markup;
using Pixie.Terminal.Devices;
using System.Text;

namespace FormattedList
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            // This example neatly prints a "hello world" title,
            // followed by five paragraphs of Lorem Ipsum.

            // First, acquire a terminal log. You should acquire
            // a log once and then re-use it in your application.
            var log = TerminalLog.AcquireStandardOutput();

            // Slightly more advanced topic (you probably won't
            // need this for a normal program):
            //
            // The `TerminalLog.AcquireStandard{Error,Output}()`
            // pick some smart default values based on whether
            // output is redirected and the terminal's
            // capabilities. But it's entirely possible to
            // exercise some more more control over the log
            // acquisition process.
            if (args.Length > 0)
            {
                if (args[0] == "fancy")
                {
                    // Use ANSI control sequences and UTF-8.
                    log = TerminalLog.Acquire(
                        new TextWriterTerminal(
                            Console.Out,
                            80,
                            new AnsiStyleManager(Console.Out),
                            Encoding.UTF8));
                }
                else if (args[0] == "simple")
                {
                    // Use the System.Console style methods and ASCII.
                    log = TerminalLog.Acquire(
                        new TextWriterTerminal(
                            Console.Out,
                            80,
                            new ConsoleStyleManager(),
                            Encoding.ASCII));
                }
            }

            // Write an entry to the log that contains the things
            // we would like to print.
            log.Log(
                new LogEntry(
                    Severity.Info,
                    new MarkupNode[]
                    {
                        // Create a title, underline it and make it green because why not.
                        new Title(
                            DecorationSpan.MakeUnderlined(
                                new ColorSpan("Hello world", Colors.Green))),

                        // Create a word-wrapped bulleted list with a uniform margin of four.
                        new WrapBox(
                            new BulletedList(
                                LoremIpsum
                                    .Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select<string, MarkupNode>(QuoteString)
                                    .ToArray<MarkupNode>(),
                                true),
                            WrappingStrategy.Word,
                            4)
                    }));
        }

        private static MarkupNode QuoteString(string text)
        {
            // Quote each string using double quotes.
            //
            // On terminals that support them, Unicode quotes will be
            // used to quote text. On terminals that don't, the
            // rendering engine will use regular ASCII quotes.
            return new Quotation(text.Trim(), 2);
        }

        private const string LoremIpsum = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Curabitur tempor ornare luctus. Etiam a vulputate tellus. Nullam ultrices ante ac metus tempor elementum. Nam facilisis sit amet massa sit amet lacinia. Phasellus hendrerit ultrices consectetur. Praesent dignissim libero sapien, nec ullamcorper sem malesuada sit amet. Nullam rhoncus nulla eu ante congue ultricies. Duis consectetur iaculis dolor, sed egestas lectus accumsan a. Nullam orci nibh, dignissim nec nulla non, interdum luctus dui. Donec semper feugiat felis in facilisis. Cras viverra, nisi vitae pretium tincidunt, lectus leo vulputate odio, et faucibus urna lorem in urna. Nam porttitor, libero pulvinar volutpat tristique, libero tellus egestas velit, a ultrices risus eros non nisi. Ut porttitor odio vitae velit fringilla volutpat.

Curabitur efficitur neque eget leo cursus, eu tincidunt massa aliquam. Pellentesque sodales ac augue quis interdum. Sed consequat lacinia ligula, eu tincidunt diam cursus porttitor. Suspendisse mattis faucibus sem, quis cursus elit mattis id. Duis ultricies condimentum dolor, ut semper odio ornare vitae. Quisque eu nisi lacinia, suscipit quam eget, porttitor tortor. Fusce consectetur bibendum lobortis. Cras iaculis enim non risus placerat porttitor id et massa. Integer faucibus dui nisi.

Sed risus massa, blandit id pretium at, suscipit ut libero. Nulla porta metus in odio imperdiet tristique. In aliquet sed tellus eget egestas. Ut eleifend nunc eu dapibus molestie. Mauris ullamcorper scelerisque eros in facilisis. Aliquam felis neque, aliquet eget nisl in, elementum imperdiet leo. Sed ac ultricies urna, id euismod elit.

Ut ut nibh consectetur, mattis augue non, commodo ipsum. Mauris eu tellus massa. Integer non mauris scelerisque, ultrices urna eget, semper ipsum. Donec ut rhoncus arcu, quis iaculis felis. Nulla faucibus vel nunc non pharetra. Sed et mattis ante, quis ultrices erat. Proin posuere lacinia justo nec varius. Suspendisse a hendrerit tortor. Suspendisse ut orci tristique, molestie massa sed, sollicitudin urna. Pellentesque molestie lacinia mauris. Cras laoreet libero eu lectus tincidunt elementum. Sed hendrerit felis in nibh lobortis, quis tincidunt diam finibus. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Mauris dignissim, sapien vel porta porta, diam orci pharetra ante, id facilisis dui massa a magna. Lorem ipsum dolor sit amet, consectetur adipiscing elit.

Phasellus a consectetur leo. Cras et pellentesque orci. Pellentesque et risus lorem. Cras efficitur molestie lectus in laoreet. Donec lobortis sit amet lorem vel fermentum. Duis vehicula consectetur eros a lacinia. Nullam non magna at metus suscipit aliquet interdum eget massa. Proin non nunc ultricies, maximus lectus id, ornare ante. Donec elementum elit neque, id fringilla quam accumsan quis. Suspendisse faucibus tincidunt risus vel convallis. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec accumsan odio diam, sit amet semper ante malesuada ac.";
    }
}
